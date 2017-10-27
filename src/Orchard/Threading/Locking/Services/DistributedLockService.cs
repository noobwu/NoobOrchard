using Autofac;
using Orchard.Data;
using Orchard.Domain.Repositories;
using Orchard.Domain.Uow;
using Orchard.Environment;
using Orchard.Environment.Configuration;
using Orchard.Exceptions;
using Orchard.Services;
using Orchard.Threading.Locking.Records;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Orchard.Threading.Locking.Services
{

    public class DistributedLockService : Component, IDistributedLockService
    {

        private readonly IApplicationEnvironment _applicationEnvironment;
        private readonly ILifetimeScope _lifetimeScope;
        private readonly IClock _clock;
        private readonly ShellSettings _shellSettings;
        private readonly Dictionary<string, DistributedLock> _locks;
        private readonly TimeSpan _defaultRepeatInterval;

        public DistributedLockService(
            IApplicationEnvironment applicationEnvironment,
            ILifetimeScope lifetimeScope,
            IClock clock,
            ShellSettings shellSettings)
        {
            _clock = clock;
            _lifetimeScope = lifetimeScope;
            _shellSettings = shellSettings;
            _applicationEnvironment = applicationEnvironment;
            _locks = new Dictionary<string, DistributedLock>();
            _defaultRepeatInterval = TimeSpan.FromMilliseconds(500);
        }

        public bool TryAcquireLock(string name, TimeSpan? maxValidFor, TimeSpan? timeout, out IDistributedLock dLock)
        {
            try
            {
                dLock = AcquireLockInternal(name, maxValidFor, timeout, throwOnTimeout: false);

                if (dLock != null)
                {
                    Logger.DebugFormat("Successfully acquired lock '{0}'.", name);
                    return true;
                }

                Logger.WarnFormat("Failed to acquire lock '{0}' within the specified timeout ({1}).", name, timeout);
            }
            catch (Exception ex)
            {
                Logger.WarnFormat(ex, "Error while trying to acquire lock '{0}'.", name);
                // TODO: Is it correct to not throw here? Should we instead ONLY swallow TimeoutException?
            }

            dLock = null;
            return false;
        }

        public IDistributedLock AcquireLock(string name, TimeSpan? maxValidFor, TimeSpan? timeout)
        {
            try
            {
                DistributedLock result = AcquireLockInternal(name, maxValidFor, timeout, throwOnTimeout: true);
                Logger.DebugFormat("Successfully acquired lock '{0}'.", name);
                return result;
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat(ex, "Error while trying to acquire lock '{0}'.", name);
                throw;
            }
        }

        private DistributedLock AcquireLockInternal(string name, TimeSpan? maxValidFor, TimeSpan? timeout, bool throwOnTimeout)
        {
            var internalName = GetInternalLockName(name);
            var monitorTimeout = timeout ?? TimeSpan.FromMilliseconds(-1); // -1 ms is .NET magic number for "infinite".
            var monitorObj = String.Intern(String.Format("{0}:{1}", _applicationEnvironment.GetEnvironmentIdentifier(), internalName));

            if (!Monitor.TryEnter(monitorObj, monitorTimeout))
            {
                Logger.DebugFormat("Could not enter local monitor for lock '{0}' within the specified timeout ({1}).", internalName, timeout);

                if (throwOnTimeout)
                    throw new TimeoutException(String.Format("Failed to acquire lock '{0}' within the specified timeout ({1}).", internalName, timeout));

                return null;
            }

            Logger.DebugFormat("Successfully entered local monitor for lock '{0}'.", internalName);

            try
            {

                // If there's already a distributed lock object in our dictionary, that means
                // this acquisition is a reentrance. Use the existing lock object from the
                // dictionary but increment its count.
                if (_locks.TryGetValue(monitorObj, out DistributedLock dLock))
                {
                    Logger.DebugFormat("Current thread is re-entering lock '{0}'; incrementing count.", internalName);
                    dLock.Increment();
                }
                else
                {
                    // No distributed lock object existed in our dictionary. Try to take ownership
                    // of database record until timeout expires, and if successful create a distributed
                    // lock object and add it to our dictionary.
                    var success = RepeatUntilTimeout(timeout, _defaultRepeatInterval, () =>
                    {
                        if (EnsureDistributedLockRecord(internalName, maxValidFor))
                        {
                            Logger.DebugFormat("Record for lock '{0}' already owned by current machine or was successfully created; creating lock object.", internalName);

                            dLock = new DistributedLock(name, internalName, releaseLockAction: () =>
                            {
                                Monitor.Exit(monitorObj);
                                DeleteDistributedLockRecord(internalName);
                            });

                            _locks.Add(monitorObj, dLock);
                            return true;
                        }

                        return false;
                    });

                    if (!success)
                    {
                        Logger.DebugFormat("Record for lock '{0}' could not be created for current machine within the specified timeout ({1}).", internalName, timeout);

                        if (throwOnTimeout)
                            throw new TimeoutException(String.Format("Failed to acquire lock '{0}' within the specified timeout ({1}).", internalName, timeout));

                        return null;
                    }
                }

                return dLock;
            }
            catch (Exception ex)
            {
                Monitor.Exit(monitorObj);

                Logger.ErrorFormat(ex, "An error occurred while trying to acquire lock '{0}'.", internalName);
                throw;
            }
        }

        private bool EnsureDistributedLockRecord(string internalName, TimeSpan? maxValidFor)
        {
            var environmentIdentifier = _applicationEnvironment.GetEnvironmentIdentifier();
            var hasLockRecord = false;

            ExecuteOnSeparateTransaction(repository =>
            {
                // Try to find a valid lock record in the database.
                var records = repository.GetAll().Where(x => x.Name == internalName).ToList();
                var record = repository.Single(x => x.ValidUntilUtc == null || x.ValidUntilUtc >= _clock.UtcNow);
                if (record == null)
                {

                    // No record matched the criteria, but at least one expired record with the specified name was found.
                    // Delete the expired records before creating a new one. In theory no more than one record can exist
                    // due to the unique key constraint on the 'Name' column, it won't hurt to work on a collection.
                    repository.Delete(x => x.Name == internalName);
                    // No valid record existed, so we're good to create a new one.
                    Logger.DebugFormat("No valid record was found for lock '{0}'; creating a new record.", internalName);
                    repository.Insert(new DistributedLockRecord
                    {
                        Name = internalName,
                        MachineName = environmentIdentifier,
                        CreatedUtc = _clock.UtcNow,
                        ValidUntilUtc = maxValidFor.HasValue ? _clock.UtcNow + maxValidFor.Value : default(DateTime?)
                    });

                    hasLockRecord = true;
                }
                else if (record.MachineName == environmentIdentifier)
                {
                    // Existing lock was for correct machine name => lock record exists.
                    Logger.DebugFormat("Found a valid record for lock '{0}' and current local machine name '{1}'.", internalName, environmentIdentifier);
                    hasLockRecord = true;
                }
            });

            return hasLockRecord;
        }

        private void DeleteDistributedLockRecord(string internalName)
        {
            try
            {
                ExecuteOnSeparateTransaction(repository =>
                {
                    var record = repository.Single(x => x.Name == internalName);
                    if (record == null)
                        throw new Exception(String.Format("No record could be found in the database for lock '{0}'.", internalName));
                    repository.Delete(record);
                    Logger.DebugFormat("Successfully deleted record for lock '{0}'.", internalName);
                });
            }
            catch (Exception ex)
            {
                if (ex.IsFatal())
                    throw;
                Logger.WarnFormat(ex, "An error occurred while deleting record for lock '{0}'.", internalName);
            }
        }

        private bool RepeatUntilTimeout(TimeSpan? timeout, TimeSpan repeatInterval, Func<bool> action)
        {
            bool success;

            var waitedTime = TimeSpan.Zero;
            while (!(success = action()) && (!timeout.HasValue || waitedTime < timeout.Value))
            {
                Task.Delay(repeatInterval).Wait();
                waitedTime += repeatInterval;
            }

            return success;
        }

        private void ExecuteOnSeparateTransaction(Action<IRepository<DistributedLockRecord>> action)
        {
            if (action == null)
                throw new ArgumentNullException();

            using (var childLifetimeScope = _lifetimeScope.BeginLifetimeScope())
            {
                var repository = childLifetimeScope.Resolve<IRepository<DistributedLockRecord>>();
                var transactionManager = childLifetimeScope.Resolve<ITransactionManager>();
                transactionManager.Begin(IsolationLevel.ReadCommitted);
                action(repository);
                transactionManager.Commit();
            }
        }

        private string GetInternalLockName(string name)
        {
            // Prefix the requested lock name by a constant and the tenant name.
            return String.Format("DistributedLock:{0}:{1}", _shellSettings.Name, name);
        }
    }
}
