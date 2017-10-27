using Nito.AsyncEx;
using Orchard.Logging;
using Orchard.Threading.Tasks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Orchard.Utility
{
    public class ScheduledTimer : IDisposable
    {
        private DateTime _next = DateTime.MaxValue;
        private DateTime _last = DateTime.MinValue;
        private readonly Timer _timer;
        private readonly ILogger _logger;
        private readonly Func<Task<DateTime?>> _timerCallback;
        private readonly TimeSpan _minimumInterval;
        private readonly AsyncLock _lock = new AsyncLock();
        private bool _isRunning = false;
        private bool _shouldRunAgainImmediately = false;

        public ScheduledTimer(Func<Task<DateTime?>> timerCallback, TimeSpan? dueTime = null, TimeSpan? minimumIntervalTime = null, ILoggerFactory loggerFactory = null)
        {
            _logger = loggerFactory.GetLogger(typeof(ScheduledTimer));
            _timerCallback = timerCallback ?? throw new ArgumentNullException(nameof(timerCallback));
            _minimumInterval = minimumIntervalTime ?? TimeSpan.Zero;

            int dueTimeMs = dueTime.HasValue ? (int)dueTime.Value.TotalMilliseconds : Timeout.Infinite;
            _timer = new Timer(s => RunCallbackAsync().GetAwaiter().GetResult(), null, dueTimeMs, Timeout.Infinite);
        }

        public void ScheduleNext(DateTime? utcDate = null)
        {
            var utcNow = SystemClock.UtcNow;
            if (!utcDate.HasValue || utcDate.Value < utcNow)
                utcDate = utcNow;

            _logger.Debug($"ScheduleNext called: value={utcDate.Value:O}");
            if (utcDate == DateTime.MaxValue)
            {
                _logger.Debug("Ignoring MaxValue");
                return;
            }

            // already have an earlier scheduled time
            if (_next > utcNow && utcDate > _next)
            {
                _logger.Debug($"Ignoring because already scheduled for earlier time: {utcDate.Value.Ticks} Next: {_next.Ticks}");
                return;
            }

            // ignore duplicate times
            if (_next == utcDate)
            {
                _logger.Debug("Ignoring because already scheduled for same time");
                return;
            }

            using (_lock.Lock())
            {
                // already have an earlier scheduled time
                if (_next > utcNow && utcDate > _next)
                {
                    _logger.Debug($"Ignoring because already scheduled for earlier time: {utcDate.Value.Ticks} Next: {_next.Ticks}");
                    return;
                }

                // ignore duplicate times
                if (_next == utcDate)
                {
                    _logger.Debug("Ignoring because already scheduled for same time");
                    return;
                }

                int delay = Math.Max((int)Math.Ceiling(utcDate.Value.Subtract(utcNow).TotalMilliseconds), 0);
                _next = utcDate.Value;
                if (_last == DateTime.MinValue)
                    _last = _next;

                _logger.Debug($"Scheduling next: delay={delay}");
                _timer.Change(delay, Timeout.Infinite);
            }
        }

        private async Task RunCallbackAsync()
        {
            if (_isRunning)
            {
                _logger.Debug("Exiting run callback because its already running, will run again immediately.");
                _shouldRunAgainImmediately = true;
                return;
            }

            _logger.Debug("Starting RunCallbackAsync");
            using (await _lock.LockAsync().AnyContext())
            {
                if (_isRunning)
                {
                    _logger.Debug("Exiting run callback because its already running, will run again immediately.");
                    _shouldRunAgainImmediately = true;
                    return;
                }

                _last = SystemClock.UtcNow;
            }

            try
            {
                _isRunning = true;
                DateTime? next = null;

                try
                {
                    next = await _timerCallback().AnyContext();
                }
                catch (Exception ex)
                {
                    _logger.Error( $"Error running scheduled timer callback: {ex.Message}",ex);
                    _shouldRunAgainImmediately = true;
                }

                if (_minimumInterval > TimeSpan.Zero)
                {
                    _logger.Debug($"Sleeping for minimum interval: {_minimumInterval}");
                    await SystemClock.SleepAsync(_minimumInterval).AnyContext();
                    _logger.Debug("Finished sleeping");
                }

                var nextRun = SystemClock.UtcNow.AddMilliseconds(10);
                if (_shouldRunAgainImmediately || next.HasValue && next.Value <= nextRun)
                    ScheduleNext(nextRun);
                else if (next.HasValue)
                    ScheduleNext(next.Value);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error running schedule next callback: {ex.Message}",ex);
            }
            finally
            {
                _isRunning = false;
                _shouldRunAgainImmediately = false;
            }

            _logger.Debug("Finished RunCallbackAsync");
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}