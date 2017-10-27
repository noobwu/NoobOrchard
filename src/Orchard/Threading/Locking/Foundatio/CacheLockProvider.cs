using Nito.AsyncEx;
using Orchard.Caching;
using Orchard.Logging;
using Orchard.Messaging;
using Orchard.Threading.Tasks;
using Orchard.Utility;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Orchard.Threading.Locking
{
    public class CacheLockProvider : ILockProvider
    {
        private readonly ICacheClient _cacheClient;
        private readonly IMessageBus _messageBus;
        private readonly ConcurrentDictionary<string, AsyncAutoResetEvent> _autoResetEvents = new ConcurrentDictionary<string, AsyncAutoResetEvent>();
        private readonly AsyncLock _lock = new AsyncLock();
        private bool _isSubscribed;
        private readonly ILogger _logger;

        public CacheLockProvider(ICacheClient cacheClient, IMessageBus messageBus, ILoggerFactory loggerFactory = null)
        {
            _logger = loggerFactory.GetLogger(typeof(CacheLockProvider));
            _cacheClient = new ScopedCacheClient(cacheClient, "lock");
            _messageBus = messageBus;
        }

        private async Task EnsureTopicSubscriptionAsync()
        {
            if (_isSubscribed)
                return;

            using (await _lock.LockAsync().AnyContext())
            {
                if (_isSubscribed)
                    return;

                _logger.Debug("Subscribing to cache lock released.");
                await _messageBus.SubscribeAsync<CacheLockReleased>(OnLockReleasedAsync).AnyContext();
                _isSubscribed = true;
                _logger.Debug("Subscribed to cache lock released.");
            }
        }

        private Task OnLockReleasedAsync(CacheLockReleased msg, CancellationToken cancellationToken = default(CancellationToken))
        {
            _logger.Debug($"Got lock released message: { msg.Name}");
            if (_autoResetEvents.TryGetValue(msg.Name, out AsyncAutoResetEvent autoResetEvent))
                autoResetEvent.Set();

            return Task.CompletedTask;
        }

        public async Task<ILock> AcquireAsync(string name, TimeSpan? lockTimeout = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            _logger.Debug($"AcquireAsync Name: {name} WillWait: {!cancellationToken.IsCancellationRequested}");

            if (!cancellationToken.IsCancellationRequested)
                await EnsureTopicSubscriptionAsync().AnyContext();

            if (!lockTimeout.HasValue)
                lockTimeout = TimeSpan.FromMinutes(20);

            bool allowLock = false;

            do
            {
                bool gotLock = false;

                try
                {
                    if (lockTimeout.Value == TimeSpan.Zero) // no lock timeout
                        gotLock = await _cacheClient.AddAsync(name, SystemClock.UtcNow).AnyContext();
                    else
                        gotLock = await _cacheClient.AddAsync(name, SystemClock.UtcNow, lockTimeout.Value).AnyContext();
                }
                catch { }

                if (gotLock)
                {
                    allowLock = true;
                    _logger.Debug($"Acquired lock: {name}");

                    break;
                }

                _logger.Debug($"Failed to acquire lock: {name}");
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.Debug("Cancellation requested");
                    break;
                }

                var keyExpiration = SystemClock.UtcNow.Add(await _cacheClient.GetExpirationAsync(name).AnyContext() ?? TimeSpan.Zero);
                var delayAmount = keyExpiration.Subtract(SystemClock.UtcNow).Max(TimeSpan.FromMilliseconds(50));

                _logger.DebugFormat("Delay amount: {0} Delay until: {1}", delayAmount, SystemClock.UtcNow.Add(delayAmount).ToString("mm:ss.fff"));

                var delayCancellationTokenSource = new CancellationTokenSource(delayAmount);
                var linkedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, delayCancellationTokenSource.Token).Token;

                var autoResetEvent = _autoResetEvents.GetOrAdd(name, new AsyncAutoResetEvent());
                var sw = Stopwatch.StartNew();

                try
                {
                    await autoResetEvent.WaitAsync(linkedCancellationToken).AnyContext();
                }
                catch (OperationCanceledException)
                {
                    if (delayCancellationTokenSource.IsCancellationRequested)
                    {
                        _logger.DebugFormat("Retrying: Delay exceeded. Cancellation requested: {0}", cancellationToken.IsCancellationRequested);
                        continue;
                    }
                }
                finally
                {
                    sw.Stop();
                    _logger.Debug($"Lock {name} waited { sw.ElapsedMilliseconds }ms");
                }
            } while (!cancellationToken.IsCancellationRequested);

            if (cancellationToken.IsCancellationRequested)
                _logger.Debug("Cancellation requested.");

            if (!allowLock)
                return null;

            _logger.Debug($"Returning lock: {name}");
            return new DisposableLock(name, this, _logger);
        }

        public async Task<bool> IsLockedAsync(string name)
        {
            var result = await Run.WithRetriesAsync(() => _cacheClient.GetAsync<object>(name), logger: _logger).AnyContext();
            return result.HasValue;
        }

        public async Task ReleaseAsync(string name)
        {
            _logger.Debug($"ReleaseAsync Start: {name}");

            await Run.WithRetriesAsync(() => _cacheClient.RemoveAsync(name), 15, logger: _logger).AnyContext();
            await _messageBus.PublishAsync(new CacheLockReleased { Name = name }).AnyContext();

            _logger.Debug($"ReleaseAsync Complete: {name}");
        }

        public Task RenewAsync(string name, TimeSpan? lockExtension = null)
        {
            _logger.DebugFormat("RenewAsync: {0}", name);
            if (!lockExtension.HasValue)
                lockExtension = TimeSpan.FromMinutes(20);

            return Run.WithRetriesAsync(() => _cacheClient.SetExpirationAsync(name, lockExtension.Value));
        }
    }

    internal class CacheLockReleased
    {
        public string Name { get; set; }
    }
}