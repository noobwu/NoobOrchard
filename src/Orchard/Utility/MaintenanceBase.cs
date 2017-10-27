using Orchard.Logging;
using System;
using System.Threading.Tasks;

namespace Orchard.Utility
{
    public class MaintenanceBase : IDisposable {
        private ScheduledTimer _maintenanceTimer;
        private readonly ILoggerFactory _loggerFactory;
        protected readonly ILogger _logger;

        public MaintenanceBase(ILoggerFactory loggerFactory) {
            if (loggerFactory == null)
            {
                loggerFactory = new NullLoggerFactory();
            }
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.GetLogger(GetType());
        }

        protected void InitializeMaintenance(TimeSpan? dueTime = null, TimeSpan? intervalTime = null) {
            _maintenanceTimer = new ScheduledTimer(DoMaintenanceAsync, dueTime, intervalTime, _loggerFactory);
        }

        protected void ScheduleNextMaintenance(DateTime utcDate) {
            _maintenanceTimer.ScheduleNext(utcDate);
        }

        protected virtual Task<DateTime?> DoMaintenanceAsync() {
            return Task.FromResult<DateTime?>(DateTime.MaxValue);
        }

        public virtual void Dispose() {
            _maintenanceTimer?.Dispose();
        }
    }
}
