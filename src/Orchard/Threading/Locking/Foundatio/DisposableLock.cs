using Orchard.Logging;
using Orchard.Threading.Tasks;
using System;
using System.Threading.Tasks;

namespace Orchard.Threading.Locking
{
    internal class DisposableLock : ILock {
        private readonly ILockProvider _lockProvider;
        private readonly string _name;
        private readonly ILogger _logger;

        public DisposableLock(string name, ILockProvider lockProvider, ILogger logger) {
            _logger = logger;
            _name = name;
            _lockProvider = lockProvider;
        }

        public async Task DisposeAsync() {
            _logger.DebugFormat("Disposing lock: {0}", _name);
            try {
                await _lockProvider.ReleaseAsync(_name).AnyContext();
            } catch (Exception ex) {
                _logger.Error($"Unable to release lock {_name}",ex);
            }
            _logger.DebugFormat("Disposed lock: {0}", _name);
        }

        public async Task RenewAsync(TimeSpan? lockExtension = null) {
            _logger.DebugFormat("Renewing lock: {0}", _name);
            await _lockProvider.RenewAsync(_name, lockExtension).AnyContext();
            _logger.DebugFormat("Renewed lock: {0}", _name);
        }

        public Task ReleaseAsync() {
            _logger.DebugFormat("Releasing lock: {0}", _name);
            return _lockProvider.ReleaseAsync(_name);
        }
    }
}