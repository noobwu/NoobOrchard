using NUnit.Framework;
using Orchard.Metrics;
using Orchard.Queues;
using Orchard.Tests.Queue;
using Orchard.Utility;
using System;
using System.Threading.Tasks;
#pragma warning disable AsyncFixer04 // A disposable object used in a fire & forget async call

namespace Orchard.Tests.Metrics
{
    public abstract class MetricsClientTestBase : TestWithLoggingBase {

        public abstract IMetricsClient GetMetricsClient(bool buffered = false);

        public virtual async Task CanSetGaugesAsync() {
            using (var metrics = GetMetricsClient()) {
                var stats = metrics as IMetricsClientStats;
                if (stats == null)
                    return;

                await metrics.GaugeAsync("mygauge", 12d);
                Assert.AreEqual(12d, (await stats.GetGaugeStatsAsync("mygauge")).Last);
                await metrics.GaugeAsync("mygauge", 10d);
                await metrics.GaugeAsync("mygauge", 5d);
                await metrics.GaugeAsync("mygauge", 4d);
                await metrics.GaugeAsync("mygauge", 12d);
                await metrics.GaugeAsync("mygauge", 20d);
                Assert.AreEqual(20d, (await stats.GetGaugeStatsAsync("mygauge")).Last);
            }
        }

        public virtual async Task CanIncrementCounterAsync() {
            using (var metrics = GetMetricsClient()) {
                var stats = metrics as IMetricsClientStats;
                if (stats == null)
                    return;

                await metrics.CounterAsync("c1");
                await AssertCounterAsync(stats, "c1", 1);

                await metrics.CounterAsync("c1", 5);
                await AssertCounterAsync(stats, "c1", 6);

                await metrics.GaugeAsync("g1", 2.534);
                Assert.AreEqual(2.534, await stats.GetLastGaugeValueAsync("g1"));

                await metrics.TimerAsync("t1", 50788);
                var timer = await stats.GetTimerStatsAsync("t1");
                Assert.AreEqual(1, timer.Count);

                _logger.Info((await stats.GetCounterStatsAsync("c1")).ToString());
            }
        }

        private async Task AssertCounterAsync(IMetricsClientStats client, string name, long expected) {
            await Run.WithRetriesAsync(async () => {
                long actual = await client.GetCounterCountAsync(name, SystemClock.UtcNow.Subtract(TimeSpan.FromHours(1)));
                Assert.AreEqual(expected, actual);
            }, 8, logger: _logger);
        }

        public virtual async Task CanGetBufferedQueueMetricsAsync() {
            try
            {
                using (var metrics = GetMetricsClient(true) as IBufferedMetricsClient)
                {
                    var stats = metrics as IMetricsClientStats;
                    if (stats == null)
                        return;

                    using (var behavior = new MetricsQueueBehavior<SimpleWorkItem>(metrics, reportCountsInterval: TimeSpan.FromMilliseconds(25), loggerFactory: LogFactory))
                    {
                        using (var queue = new InMemoryQueue<SimpleWorkItem>(new InMemoryQueueOptions<SimpleWorkItem> { Behaviors = new[] { behavior }, LoggerFactory = LogFactory }))
                        {
                            await queue.EnqueueAsync(new SimpleWorkItem { Id = 1, Data = "1" });
                            await SystemClock.SleepAsync(50);
                            var entry = await queue.DequeueAsync(TimeSpan.Zero);
                            await SystemClock.SleepAsync(15);
                            await entry.CompleteAsync();

                            await SystemClock.SleepAsync(100); // give queue metrics time
                            await metrics.FlushAsync();
                            var queueStats = await stats.GetQueueStatsAsync("simpleworkitem");
                            Assert.AreEqual(1, queueStats.Count.Max);
                            Assert.AreEqual(0, queueStats.Count.Last);
                            Assert.AreEqual(1, queueStats.Enqueued.Count);
                            Assert.AreEqual(1, queueStats.Dequeued.Count);
                            Assert.AreEqual(1, queueStats.Completed.Count);
                            Assert.IsTrue(queueStats.QueueTime.AverageDuration >= 45 && queueStats.QueueTime.AverageDuration <= 250);
                            Assert.IsTrue(queueStats.ProcessTime.AverageDuration >= 10 && queueStats.ProcessTime.AverageDuration <= 250);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }
            
        }

        public virtual async Task CanIncrementBufferedCounterAsync() {
            using (var metrics = GetMetricsClient(true) as IBufferedMetricsClient) {
                var stats = metrics as IMetricsClientStats;
                if (stats == null)
                    return;

                await metrics.CounterAsync("c1");
                await metrics.FlushAsync();
                var counter = await stats.GetCounterStatsAsync("c1", SystemClock.UtcNow.AddMinutes(-5), SystemClock.UtcNow);
                Assert.AreEqual(1, counter.Count);

                await metrics.CounterAsync("c1", 5);
                await metrics.FlushAsync();
                counter = await stats.GetCounterStatsAsync("c1", SystemClock.UtcNow.AddMinutes(-5), SystemClock.UtcNow);
                Assert.AreEqual(6, counter.Count);

                await metrics.GaugeAsync("g1", 5.34);
                await metrics.FlushAsync();
                var gauge = await stats.GetGaugeStatsAsync("g1", SystemClock.UtcNow.AddMinutes(-5), SystemClock.UtcNow);
                Assert.AreEqual(5.34, gauge.Last);
                Assert.AreEqual(5.34, gauge.Max);

                await metrics.GaugeAsync("g1", 2.534);
                await metrics.FlushAsync();
                gauge = await stats.GetGaugeStatsAsync("g1", SystemClock.UtcNow.AddMinutes(-5), SystemClock.UtcNow);
                Assert.AreEqual(2.534, gauge.Last);
                Assert.AreEqual(5.34, gauge.Max);

                await metrics.TimerAsync("t1", 50788);
                await metrics.FlushAsync();
                var timer = await stats.GetTimerStatsAsync("t1", SystemClock.UtcNow.AddMinutes(-5), SystemClock.UtcNow);
                Assert.AreEqual(1, timer.Count);
                Assert.AreEqual(50788, timer.TotalDuration);

                await metrics.TimerAsync("t1", 98);
                await metrics.TimerAsync("t1", 102);
                await metrics.FlushAsync();
                timer = await stats.GetTimerStatsAsync("t1", SystemClock.UtcNow.AddMinutes(-5), SystemClock.UtcNow);
                Assert.AreEqual(3, timer.Count);
                Assert.AreEqual(50788 + 98 + 102, timer.TotalDuration);
            }
        }

#pragma warning disable 4014
        public virtual async Task CanWaitForCounterAsync() {
            const string CounterName = "Test";
            using (var metrics = GetMetricsClient() as CacheBucketMetricsClientBase) {
                var stats = metrics as IMetricsClientStats;
                if (stats == null)
                    return;

                Task.Run(async () => {
                    await SystemClock.SleepAsync(TimeSpan.FromMilliseconds(50));
                    await metrics.CounterAsync(CounterName);
                });

                var task = metrics.WaitForCounterAsync(CounterName, 1, TimeSpan.FromMilliseconds(500));
                await SystemClock.SleepAsync(TimeSpan.FromMilliseconds(50));
                Assert.True(await task, "Expected at least 1 count within 500 ms");

                Task.Run(async () => {
                    await SystemClock.SleepAsync(TimeSpan.FromMilliseconds(50));
                    await metrics.CounterAsync(CounterName);
                });

                task = metrics.WaitForCounterAsync(CounterName, timeout: TimeSpan.FromMilliseconds(500));
                await SystemClock.SleepAsync(TimeSpan.FromMilliseconds(50));
                Assert.True(await task, "Expected at least 2 count within 500 ms");

                Task.Run(async () => {
                    await SystemClock.SleepAsync(TimeSpan.FromMilliseconds(50));
                    await metrics.CounterAsync(CounterName, 2);
                });

                task = metrics.WaitForCounterAsync(CounterName, 2, TimeSpan.FromMilliseconds(500));
                await SystemClock.SleepAsync(TimeSpan.FromMilliseconds(50));
                Assert.True(await task, "Expected at least 4 count within 500 ms");

                task = metrics.WaitForCounterAsync(CounterName, () => metrics.CounterAsync(CounterName), cancellationToken: TimeSpan.FromMilliseconds(500).ToCancellationToken());
                await SystemClock.SleepAsync(TimeSpan.FromMilliseconds(500));
                Assert.True(await task, "Expected at least 5 count within 500 ms");

                _logger.Info((await metrics.GetCounterStatsAsync(CounterName)).ToString());
            }
        }
#pragma warning restore 4014

        public virtual async Task CanSendBufferedMetricsAsync() {
            using (var metrics = GetMetricsClient(true) as IBufferedMetricsClient) {
                var stats = metrics as IMetricsClientStats;
                if (stats == null)
                    return;

                Parallel.For(0, 100, i => metrics.CounterAsync("c1").GetAwaiter().GetResult());

                await metrics.FlushAsync();

                var counter = await stats.GetCounterStatsAsync("c1");
                Assert.AreEqual(100, counter.Count);
            }
        }
    }
}