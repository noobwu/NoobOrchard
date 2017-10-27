using Orchard.Exceptions;
using System;
using System.Data;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Orchard.Logging;
using Orchard.Domain.Uow;
using NHibernate;

namespace Orchard.Data.NHibernate.Uow
{
    /// <summary>
    /// This handle is used for innet unit of work scopes.
    /// A inner unit of work scope actually uses outer unit of work scope
    /// and has no effect on <see cref="IUnitOfWorkCompleteHandle.Complete"/> call.
    /// But if it's not called, an exception is thrown at end of the UOW to rollback the UOW.
    /// </summary>
    public class NHibernateTransactionCompleteHandle : ITransactionCompleteHandle
    {
        private ISession _session;
        private ITransaction _transaction;
        public ILogger Logger { get; set; }
        public NHibernateTransactionCompleteHandle(ISession session, IsolationLevel isolationLevel)
        {
            _session = session;
            Logger = NullLogger.Instance;
            _transaction = _session.BeginTransaction(isolationLevel);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Commit()
        {
            if (_session != null)
            {                  // IsActive is true if the transaction hasn't been committed or rolled back
                if (_transaction != null && _transaction.IsActive)
                {
                    try
                    {
                        Logger.Debug("Committing transaction");
                        _transaction.Commit();

                    }
                    catch (Exception ex)
                    {
                        _transaction.Rollback();
                        DisposeSession();
                        throw ex;
                    }
                }
            }
            DisposeSession();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Rollback()
        {
            if (_session != null)
            {
                // IsActive is true if the transaction hasn't been committed or rolled back
                if (_transaction != null && _transaction.IsActive)
                {
                    Logger.Debug("Rolling back transaction");
                    _transaction.Rollback();
                }
            }
            DisposeSession();
        }

        private void EnsureSession()
        {
            if (_session == null)
            {
                throw new ArgumentNullException("Session can't be null, ever");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void DisposeSession()
        {
            Logger.Debug("Disposing session");
            if (_session != null)
            {
                _session.Close();
                _session.Dispose();
                _session = null;
            }
            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            DisposeSession();
        }
        private static bool HasException()
        {
            try
            {
                return Marshal.GetExceptionCode() != 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}