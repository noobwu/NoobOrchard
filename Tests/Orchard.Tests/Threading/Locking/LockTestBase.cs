using NUnit.Framework;
using Orchard.Threading.Locking;
using Orchard.Utility;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Orchard.Tests.Threading.Locking
{
    public abstract class LockTestBase : TestWithLoggingBase {
       
        protected virtual ILockProvider GetThrottlingLockProvider(int maxHits, TimeSpan period) {
            return null;
        }

        protected virtual ILockProvider GetLockProvider() {
            return null;
        }

        public virtual async Task CanAcquireAndReleaseLockAsync() {
            var locker = GetLockProvider();
            if (locker == null)
                return;

            await locker.ReleaseAsync("test");

            var lock1 = await locker.AcquireAsync("test", acquireTimeout: TimeSpan.FromMilliseconds(100), lockTimeout: TimeSpan.FromSeconds(1));

            try {
                Assert.NotNull(lock1);
                Assert.True(await locker.IsLockedAsync("test"));
                var lock2Task = locker.AcquireAsync("test", acquireTimeout: TimeSpan.FromMilliseconds(250));
                await SystemClock.SleepAsync(TimeSpan.FromMilliseconds(250));
                Assert.Null(await lock2Task);
            } finally {
                await lock1.ReleaseAsync();
            }

            Assert.False(await locker.IsLockedAsync("test"));

            int counter = 0;

            await Run.InParallelAsync(25, async i => {
                var sw = Stopwatch.StartNew();
                var lock2 = await locker.AcquireAsync("test", acquireTimeout: TimeSpan.FromSeconds(1));
                sw.Stop();

                try {
                    _logger.Debug($"Lock {i}: start");
                    string message = lock2 != null ? "Acquired" : "Unable to acquire";
                    _logger.Debug($"Lock {i}: {message} in {sw.ElapsedMilliseconds}ms");

                    Assert.NotNull(lock2);
                    Assert.True(await locker.IsLockedAsync("test"), $"Lock {i}: was acquired but is not locked");
                    Interlocked.Increment(ref counter);
                    _logger.Debug($"Lock {i}: end");
                } finally {
                    if (lock2 != null)
                        await lock2.ReleaseAsync();
                }
            });

           Assert.AreEqual(25, counter);
        }

        public virtual async Task LockWillTimeoutAsync() {
            var locker = GetLockProvider();
            if (locker == null)
                return;
            
            _logger.Info("Releasing lock");
            await locker.ReleaseAsync("test");
                
            _logger.Info("Acquiring lock #1");
            var testLock = await locker.AcquireAsync("test", lockTimeout: TimeSpan.FromMilliseconds(250));
            _logger.Info(testLock != null ? "Acquired lock #1" : "Unable to acquire lock #1");
            Assert.NotNull(testLock);

            _logger.Info("Acquiring lock #2");
            testLock = await locker.AcquireAsync("test", acquireTimeout: TimeSpan.FromMilliseconds(50));
            _logger.Info(testLock != null ? "Acquired lock #2" : "Unable to acquire lock #2");
            Assert.Null(testLock);

            _logger.Info("Acquiring lock #3");
            testLock = await locker.AcquireAsync("test", acquireTimeout: TimeSpan.FromMilliseconds(300));
            _logger.Info(testLock != null ? "Acquired lock #3" : "Unable to acquire lock #3");
            Assert.NotNull(testLock);
        }

        public virtual async Task LockOneAtATimeAsync() {
            var locker = GetLockProvider();
            if (locker == null)
                return;

            await locker.ReleaseAsync("DoLockedWork");
            int successCount = 0;
            var lockTask1 = Task.Run(async () => {
                if (await DoLockedWorkAsync(locker)) {
                    Interlocked.Increment(ref successCount);
                    _logger.Info("LockTask1 Success");
                }
            });
            var lockTask2 = Task.Run(async () => {
                if (await DoLockedWorkAsync(locker)) {
                    Interlocked.Increment(ref successCount);
                    _logger.Info("LockTask2 Success");
                }
            });
            var lockTask3 = Task.Run(async () => {
                if (await DoLockedWorkAsync(locker)) {
                    Interlocked.Increment(ref successCount);
                    _logger.Info("LockTask3 Success");
                }
            });
            var lockTask4 = Task.Run(async () => {
                if (await DoLockedWorkAsync(locker)) {
                    Interlocked.Increment(ref successCount);
                    _logger.Info("LockTask4 Success");
                }
            });

            await Task.WhenAll(lockTask1, lockTask2, lockTask3, lockTask4);
           Assert.AreEqual(1, successCount);

            await Task.Run(async () => {
                if (await DoLockedWorkAsync(locker))
                    Interlocked.Increment(ref successCount);
            });
           Assert.AreEqual(2, successCount);
        }

        private async Task<bool> DoLockedWorkAsync(ILockProvider locker) {
            return await locker.TryUsingAsync("DoLockedWork", async () => await SystemClock.SleepAsync(500), TimeSpan.FromMinutes(1), TimeSpan.Zero);
        }

        public virtual async Task WillThrottleCallsAsync() {
            const int allowedLocks = 25;

            var period = TimeSpan.FromSeconds(2);
            var locker = GetThrottlingLockProvider(allowedLocks, period);
            if (locker == null)
                return;

            var lockName = Guid.NewGuid().ToString("N").Substring(10);

            // sleep until start of throttling period
            var utcNow = SystemClock.UtcNow;
            await SystemClock.SleepAsync(utcNow.Ceiling(period) - utcNow);
            var sw = Stopwatch.StartNew();
            for (int i = 1; i <= allowedLocks; i++) {
                _logger.Info($"Allowed Locks: {i}");
                var l = await locker.AcquireAsync(lockName);
                Assert.NotNull(l);
            }
            sw.Stop();

            _logger.InfoFormat("Time to acquire {0} locks: {1}", allowedLocks, sw.Elapsed);
            Assert.True(sw.Elapsed.TotalSeconds < 1);

            sw.Restart();
            var result = await locker.AcquireAsync(lockName, acquireTimeout: TimeSpan.FromMilliseconds(350));
            sw.Stop();
            _logger.InfoFormat("Total acquire time took to attempt to get throttled lock: {0}", sw.Elapsed);
            Assert.Null(result);

            sw.Restart();
            result = await locker.AcquireAsync(lockName, acquireTimeout: TimeSpan.FromSeconds(2.0));
            sw.Stop();
            _logger.InfoFormat("Time to acquire lock: {0}", sw.Elapsed);
            Assert.NotNull(result);
        }
    }
}
