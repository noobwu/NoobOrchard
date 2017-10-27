using Nito.AsyncEx;
using NUnit.Framework;
using Orchard.Metrics;
using Orchard.Queues;
using Orchard.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Orchard.Jobs;
using Orchard.Caching;
using Orchard.Messaging;
using Orchard.Threading.Locking;
#pragma warning disable CS4014
#pragma warning disable AsyncFixer02

namespace Orchard.Tests.Queue
{
    public abstract class QueueTestBase : TestWithLoggingBase, IDisposable
    {

        protected virtual IQueue<SimpleWorkItem> GetQueue(int retries = 1, TimeSpan? workItemTimeout = null, TimeSpan? retryDelay = null, int deadLetterMaxItems = 100, bool runQueueMaintenance = true)
        {
            return null;
        }

        protected virtual async Task CleanupQueueAsync(IQueue<SimpleWorkItem> queue)
        {
            if (queue == null)
                return;

            try
            {
                await queue.DeleteQueueAsync();
            }
            catch (Exception ex)
            {
                _logger.Error("Error cleaning up queue", ex);
            }
            finally
            {
                queue.Dispose();
            }
        }

        public virtual async Task CanQueueAndDequeueWorkItemAsync()
        {
            var queue = GetQueue();
            if (queue == null)
                return;

            try
            {
                await queue.DeleteQueueAsync();
                await AssertEmptyQueueAsync(queue);

                await queue.EnqueueAsync(new SimpleWorkItem
                {
                    Data = "Hello"
                });
                Assert.AreEqual(1, (await queue.GetQueueStatsAsync()).Enqueued);

                var workItem = await queue.DequeueAsync(TimeSpan.Zero);
                Assert.NotNull(workItem);
                Assert.AreEqual("Hello", workItem.Value.Data);
                Assert.AreEqual(1, (await queue.GetQueueStatsAsync()).Dequeued);

                await workItem.CompleteAsync();
                Assert.False(workItem.IsAbandoned);
                Assert.True(workItem.IsCompleted);
                var stats = await queue.GetQueueStatsAsync();
                Assert.AreEqual(1, stats.Completed);
                Assert.AreEqual(0, stats.Queued);

            }
            finally
            {
                await CleanupQueueAsync(queue);
            }
        }

        /// <summary>
        /// When a cancelled token is passed into Dequeue, it will only try to dequeue one time and then exit.
        /// </summary>
        /// <returns></returns>
        public virtual async Task CanDequeueWithCancelledTokenAsync()
        {
            var queue = GetQueue();
            if (queue == null)
                return;

            try
            {
                await queue.DeleteQueueAsync();
                await AssertEmptyQueueAsync(queue);

                await queue.EnqueueAsync(new SimpleWorkItem
                {
                    Data = "Hello"
                });
                Assert.AreEqual(1, (await queue.GetQueueStatsAsync()).Enqueued);

                var workItem = await queue.DequeueAsync(new CancellationToken(true));
                Assert.NotNull(workItem);
                Assert.AreEqual("Hello", workItem.Value.Data);
                Assert.AreEqual(1, (await queue.GetQueueStatsAsync()).Dequeued);

                // TODO: We should verify that only one retry occurred.
                await workItem.CompleteAsync();
                var stats = await queue.GetQueueStatsAsync();
                Assert.AreEqual(1, stats.Completed);
                Assert.AreEqual(0, stats.Queued);
            }
            finally
            {
                await CleanupQueueAsync(queue);
            }
        }

        public virtual async Task CanDequeueEfficientlyAsync()
        {
            const int iterations = 100;

            var queue = GetQueue(runQueueMaintenance: false);
            if (queue == null)
                return;

            try
            {
                await queue.DeleteQueueAsync();
                await AssertEmptyQueueAsync(queue);
                await queue.EnqueueAsync(new SimpleWorkItem { Data = "Initialize queue to create more accurate metrics" });
                Assert.NotNull(await queue.DequeueAsync(TimeSpan.FromSeconds(1)));

                using (var metrics = new InMemoryMetricsClient(new InMemoryMetricsClientOptions() { LoggerFactory=LogFactory}))
                {
                    queue.AttachBehavior(new MetricsQueueBehavior<SimpleWorkItem>(metrics, reportCountsInterval: TimeSpan.FromMilliseconds(100), loggerFactory: LogFactory));

                    Task.Run(async () =>
                    {
                        _logger.Debug("Starting enqueue loop.");
                        for (int index = 0; index < iterations; index++)
                        {
                            await SystemClock.SleepAsync(RandomData.GetInt(10, 30));
                            await queue.EnqueueAsync(new SimpleWorkItem { Data = "Hello" });
                        }
                        _logger.Debug("Finished enqueuing.");
                    });

                    _logger.Debug("Starting dequeue loop.");
                    for (int index = 0; index < iterations; index++)
                    {
                        var item = await queue.DequeueAsync(TimeSpan.FromSeconds(3));
                        Assert.NotNull(item);
                        await item.CompleteAsync();
                    }
                    _logger.Debug("Finished dequeuing.");

                    await metrics.FlushAsync();
                    var timing = await metrics.GetTimerStatsAsync("simpleworkitem.queuetime");
                    _logger.Debug($"AverageDuration: {timing.AverageDuration}");
                    Assert.IsTrue(timing.AverageDuration >= 0 && timing.AverageDuration <= 75);
                }
            }
            finally
            {
                await CleanupQueueAsync(queue);
            }
        }

        public virtual async Task CanResumeDequeueEfficientlyAsync()
        {
            const int iterations = 10;

            var queue = GetQueue(runQueueMaintenance: false);
            if (queue == null)
                return;

            try
            {
                await queue.DeleteQueueAsync();
                await AssertEmptyQueueAsync(queue);

                using (var metrics = new InMemoryMetricsClient(new InMemoryMetricsClientOptions()))
                {
                    for (int index = 0; index < iterations; index++)
                        await queue.EnqueueAsync(new SimpleWorkItem { Data = "Hello" });

                    using (var secondQueue = GetQueue(runQueueMaintenance: false))
                    {
                        secondQueue.AttachBehavior(new MetricsQueueBehavior<SimpleWorkItem>(metrics, reportCountsInterval: TimeSpan.FromMilliseconds(100), loggerFactory: LogFactory));

                        _logger.Debug("Starting dequeue loop.");
                        for (int index = 0; index < iterations; index++)
                        {
                            _logger.Debug($"[{index}] Calling Dequeue");
                            var item = await secondQueue.DequeueAsync(TimeSpan.FromSeconds(3));
                            Assert.NotNull(item);
                            await item.CompleteAsync();
                        }

                        await metrics.FlushAsync(); // This won't flush metrics queue behaviors
                        var timing = await metrics.GetTimerStatsAsync("simpleworkitem.queuetime");
                        _logger.Debug($"TotalDuration: {timing.TotalDuration} AverageDuration: {timing.AverageDuration}");
                        Assert.IsTrue(timing.AverageDuration >= 0 && timing.AverageDuration <= 75);
                    }
                }
            }
            finally
            {
                await CleanupQueueAsync(queue);
            }
        }

        public virtual async Task CanQueueAndDequeueMultipleWorkItemsAsync()
        {
            var queue = GetQueue();
            if (queue == null)
                return;

            try
            {
                await queue.DeleteQueueAsync();
                await AssertEmptyQueueAsync(queue);

                const int workItemCount = 25;
                for (int i = 0; i < workItemCount; i++)
                {
                    await queue.EnqueueAsync(new SimpleWorkItem
                    {
                        Data = "Hello"
                    });
                }
                Assert.AreEqual(workItemCount, (await queue.GetQueueStatsAsync()).Queued);

                var sw = Stopwatch.StartNew();
                for (int i = 0; i < workItemCount; i++)
                {
                    var workItem = await queue.DequeueAsync(TimeSpan.FromSeconds(5));
                    Assert.NotNull(workItem);
                    Assert.AreEqual("Hello", workItem.Value.Data);
                    await workItem.CompleteAsync();
                }
                sw.Stop();
                _logger.DebugFormat("Time {0}", sw.Elapsed);
                Assert.LessOrEqual(sw.Elapsed.TotalSeconds, 5);

                var stats = await queue.GetQueueStatsAsync();
                Assert.AreEqual(workItemCount, stats.Dequeued);
                Assert.AreEqual(workItemCount, stats.Completed);
                Assert.AreEqual(0, stats.Queued);
            }
            finally
            {
                await CleanupQueueAsync(queue);
            }
        }

        public virtual async Task WillNotWaitForItemAsync()
        {
            var queue = GetQueue();
            if (queue == null)
                return;

            try
            {
                await queue.DeleteQueueAsync();
                await AssertEmptyQueueAsync(queue);

                var sw = Stopwatch.StartNew();
                var workItem = await queue.DequeueAsync(TimeSpan.Zero);
                sw.Stop();
                _logger.DebugFormat("Time {0}", sw.Elapsed);
                Assert.Null(workItem);
                Assert.LessOrEqual(sw.Elapsed.TotalMilliseconds, 100);
            }
            finally
            {
                await CleanupQueueAsync(queue);
            }
        }

        public virtual async Task WillWaitForItemAsync()
        {
            var queue = GetQueue();
            if (queue == null)
                return;

            try
            {
                await queue.DeleteQueueAsync();
                await AssertEmptyQueueAsync(queue);

                var sw = Stopwatch.StartNew();
                var workItem = await queue.DequeueAsync(TimeSpan.FromMilliseconds(100));
                sw.Stop();
                _logger.DebugFormat("Time {0}", sw.Elapsed);
                Assert.Null(workItem);
                Assert.True(sw.Elapsed > TimeSpan.FromMilliseconds(100));

                Task.Run(async () =>
                {
                    await SystemClock.SleepAsync(500);
                    await queue.EnqueueAsync(new SimpleWorkItem
                    {
                        Data = "Hello"
                    });
                });

                sw.Restart();
                workItem = await queue.DequeueAsync(TimeSpan.FromSeconds(1));
                sw.Stop();
                _logger.DebugFormat("Time {0}", sw.Elapsed);
                Assert.True(sw.Elapsed > TimeSpan.FromMilliseconds(400));
                Assert.NotNull(workItem);
                await workItem.CompleteAsync();

            }
            finally
            {
                await CleanupQueueAsync(queue);
            }
        }

        public virtual async Task DequeueWaitWillGetSignaledAsync()
        {
            var queue = GetQueue();
            if (queue == null)
                return;

            try
            {
                await queue.DeleteQueueAsync();
                await AssertEmptyQueueAsync(queue);

                Task.Run(async () =>
                {
                    await SystemClock.SleepAsync(250);
                    await queue.EnqueueAsync(new SimpleWorkItem
                    {
                        Data = "Hello"
                    });
                });

                var sw = Stopwatch.StartNew();
                var workItem = await queue.DequeueAsync(TimeSpan.FromSeconds(2));
                sw.Stop();
                _logger.DebugFormat("Time {0}", sw.Elapsed);
                Assert.NotNull(workItem);
                Assert.LessOrEqual(sw.Elapsed.TotalSeconds, 2);
            }
            finally
            {
                await CleanupQueueAsync(queue);
            }
        }

        public virtual async Task CanUseQueueWorkerAsync()
        {
            var queue = GetQueue();
            if (queue == null)
                return;

            try
            {
                await queue.DeleteQueueAsync();
                await AssertEmptyQueueAsync(queue);

                var resetEvent = new AsyncManualResetEvent(false);
                await queue.StartWorkingAsync(async w =>
                {
                    Assert.AreEqual("Hello", w.Value.Data);
                    await w.CompleteAsync();
                    resetEvent.Set();
                });

                await queue.EnqueueAsync(new SimpleWorkItem
                {
                    Data = "Hello"
                });

                await resetEvent.WaitAsync();
                var stats = await queue.GetQueueStatsAsync();
                Assert.AreEqual(1, stats.Completed);
                Assert.AreEqual(0, stats.Queued);
                Assert.AreEqual(0, stats.Errors);
            }
            finally
            {
                await CleanupQueueAsync(queue);
            }
        }

        public virtual async Task CanHandleErrorInWorkerAsync()
        {
            var queue = GetQueue(retries: 0);
            if (queue == null)
                return;

            try
            {
                await queue.DeleteQueueAsync();
                await AssertEmptyQueueAsync(queue);

                using (var metrics = new InMemoryMetricsClient(new InMemoryMetricsClientOptions { Buffered = false, LoggerFactory = LogFactory }))
                {
                    queue.AttachBehavior(new MetricsQueueBehavior<SimpleWorkItem>(metrics, reportCountsInterval: TimeSpan.FromMilliseconds(100), loggerFactory: LogFactory));
                    await queue.StartWorkingAsync(w =>
                    {
                        _logger.Debug("WorkAction");
                        Assert.AreEqual("Hello", w.Value.Data);
                        throw new Exception();
                    });

                    var resetEvent = new AsyncManualResetEvent(false);
                    using (queue.Abandoned.AddSyncHandler((o, args) => resetEvent.Set()))
                    {
                        await queue.EnqueueAsync(new SimpleWorkItem { Data = "Hello" });
                        await resetEvent.WaitAsync();

                        await SystemClock.SleepAsync(100); // give time for the stats to reflect the changes.
                        var stats = await queue.GetQueueStatsAsync();
                        _logger.Info($"Completed: { stats.Completed} Errors: {stats.Errors} Deadletter: {  stats.Deadletter} Working: { stats.Working} ");
                        Assert.AreEqual(0, stats.Completed);
                        Assert.AreEqual(1, stats.Errors);
                        Assert.AreEqual(1, stats.Deadletter);
                    }
                }
            }
            finally
            {
                await CleanupQueueAsync(queue);
            }
        }

        public virtual async Task WorkItemsWillTimeoutAsync()
        {
            var queue = GetQueue(retryDelay: TimeSpan.Zero, workItemTimeout: TimeSpan.FromMilliseconds(50));
            if (queue == null)
                return;

            try
            {
                await queue.DeleteQueueAsync();
                await AssertEmptyQueueAsync(queue);

                await queue.EnqueueAsync(new SimpleWorkItem
                {
                    Data = "Hello"
                });
                var workItem = await queue.DequeueAsync(TimeSpan.Zero);
                Assert.NotNull(workItem);
                Assert.AreEqual("Hello", workItem.Value.Data);
                await SystemClock.SleepAsync(TimeSpan.FromSeconds(1));

                // wait for the task to be auto abandoned

                var sw = Stopwatch.StartNew();
                workItem = await queue.DequeueAsync(TimeSpan.FromSeconds(5));
                sw.Stop();
                _logger.DebugFormat("Time {0}", sw.Elapsed);
                Assert.NotNull(workItem);
                await workItem.CompleteAsync();
                Assert.AreEqual(0, (await queue.GetQueueStatsAsync()).Queued);
            }
            finally
            {
                await CleanupQueueAsync(queue);
            }
        }

        public virtual async Task WorkItemsWillGetMovedToDeadletterAsync()
        {
            var queue = GetQueue(retryDelay: TimeSpan.Zero);
            if (queue == null)
                return;

            try
            {
                await queue.DeleteQueueAsync();
                await AssertEmptyQueueAsync(queue);

                await queue.EnqueueAsync(new SimpleWorkItem
                {
                    Data = "Hello"
                });
                var workItem = await queue.DequeueAsync(TimeSpan.Zero);
                Assert.AreEqual("Hello", workItem.Value.Data);
                Assert.AreEqual(1, (await queue.GetQueueStatsAsync()).Dequeued);

                await workItem.AbandonAsync();
                Assert.AreEqual(1, (await queue.GetQueueStatsAsync()).Abandoned);

                // work item should be retried 1 time.
                workItem = await queue.DequeueAsync(TimeSpan.FromSeconds(5));
                Assert.NotNull(workItem);
                Assert.AreEqual("Hello", workItem.Value.Data);
                Assert.AreEqual(2, (await queue.GetQueueStatsAsync()).Dequeued);

                await workItem.AbandonAsync();

                // work item should be moved to deadletter _queue after retries.
                var stats = await queue.GetQueueStatsAsync();
                Assert.AreEqual(1, stats.Deadletter);
                Assert.AreEqual(2, stats.Abandoned);
            }
            finally
            {
                await CleanupQueueAsync(queue);
            }
        }

        public virtual async Task CanAutoCompleteWorkerAsync()
        {
            var queue = GetQueue();
            if (queue == null)
                return;

            try
            {
                await queue.DeleteQueueAsync();
                await AssertEmptyQueueAsync(queue);

                var resetEvent = new AsyncManualResetEvent(false);
                await queue.StartWorkingAsync(w =>
                {
                    Assert.AreEqual("Hello", w.Value.Data);
                    return Task.CompletedTask;
                }, true);

                using (queue.Completed.AddSyncHandler((s, e) => { resetEvent.Set(); }))
                {
                    await queue.EnqueueAsync(new SimpleWorkItem { Data = "Hello" });

                    Assert.AreEqual(1, (await queue.GetQueueStatsAsync()).Enqueued);
                    await resetEvent.WaitAsync();

                    var stats = await queue.GetQueueStatsAsync();
                    Assert.AreEqual(0, stats.Queued);
                    Assert.AreEqual(0, stats.Errors);
                    Assert.AreEqual(1, stats.Completed);
                }
            }
            finally
            {
                await CleanupQueueAsync(queue);
            }
        }

        public virtual async Task CanHaveMultipleQueueInstancesAsync()
        {
            var queue = GetQueue(retries: 0, retryDelay: TimeSpan.Zero);
            if (queue == null)
                return;

            try
            {
                await queue.DeleteQueueAsync();
                await AssertEmptyQueueAsync(queue);

                const int workItemCount = 500;
                const int workerCount = 3;
                var countdown = new AsyncCountdownEvent(workItemCount);
                var info = new WorkInfo();
                var workers = new List<IQueue<SimpleWorkItem>> { queue };

                try
                {
                    for (int i = 0; i < workerCount; i++)
                    {
                        var q = GetQueue(retries: 0, retryDelay: TimeSpan.Zero);
                        _logger.DebugFormat("Queue Id: {0}, I: {1}", q.QueueId, i);
                        await q.StartWorkingAsync(w => DoWorkAsync(w, countdown, info));
                        workers.Add(q);
                    }

                    await Run.InParallelAsync(workItemCount, async i =>
                    {
                        string id = await queue.EnqueueAsync(new SimpleWorkItem
                        {
                            Data = "Hello",
                            Id = i
                        });
                        _logger.DebugFormat("Enqueued Index: {0} Id: {1}", i, id);
                    });

                    await countdown.WaitAsync();
                    await SystemClock.SleepAsync(50);
                    _logger.DebugFormat("Completed: {0} Abandoned: {1} Error: {2}",
                        info.CompletedCount,
                        info.AbandonCount,
                        info.ErrorCount);


                    _logger.Info($"Work Info Stats: Completed: {info.CompletedCount} Abandoned: {info.AbandonCount} Error: {info.ErrorCount}");
                    Assert.AreEqual(workItemCount, info.CompletedCount + info.AbandonCount + info.ErrorCount);

                    // In memory queue doesn't share state.
                    if (queue.GetType() == typeof(InMemoryQueue<SimpleWorkItem>))
                    {
                        var stats = await queue.GetQueueStatsAsync();
                        Assert.AreEqual(0, stats.Working);
                        Assert.AreEqual(0, stats.Timeouts);
                        Assert.AreEqual(workItemCount, stats.Enqueued);
                        Assert.AreEqual(workItemCount, stats.Dequeued);
                        Assert.AreEqual(info.CompletedCount, stats.Completed);
                        Assert.AreEqual(info.ErrorCount, stats.Errors);
                        Assert.AreEqual(info.AbandonCount, stats.Abandoned - info.ErrorCount);
                        Assert.AreEqual(info.AbandonCount + stats.Errors, stats.Deadletter);
                    }
                    else
                    {
                        var workerStats = new List<QueueStats>();
                        for (int i = 0; i < workers.Count; i++)
                        {
                            var stats = await workers[i].GetQueueStatsAsync();
                            _logger.Info($"Worker#{i} Working: {stats.Working} Completed: { stats.Completed} Abandoned: {stats.Abandoned} Error: { stats.Errors} Deadletter: {stats.Deadletter}");
                            workerStats.Add(stats);
                        }

                        Assert.AreEqual(info.CompletedCount, workerStats.Sum(s => s.Completed));
                        Assert.AreEqual(info.ErrorCount, workerStats.Sum(s => s.Errors));
                        Assert.AreEqual(info.AbandonCount, workerStats.Sum(s => s.Abandoned) - info.ErrorCount);
                        Assert.AreEqual(info.AbandonCount + workerStats.Sum(s => s.Errors), (workerStats.LastOrDefault()?.Deadletter ?? 0));
                    }
                }
                finally
                {
                    foreach (var q in workers)
                        await CleanupQueueAsync(q);
                }
            }
            finally
            {
                await CleanupQueueAsync(queue);
            }
        }

        public virtual async Task CanDelayRetryAsync()
        {
            var queue = GetQueue(workItemTimeout: TimeSpan.FromMilliseconds(500), retryDelay: TimeSpan.FromSeconds(1));
            if (queue == null)
                return;

            try
            {
                await queue.DeleteQueueAsync();
                await AssertEmptyQueueAsync(queue);

                await queue.EnqueueAsync(new SimpleWorkItem
                {
                    Data = "Hello"
                });

                var workItem = await queue.DequeueAsync(TimeSpan.Zero);
                Assert.NotNull(workItem);
                Assert.AreEqual("Hello", workItem.Value.Data);

                var startTime = SystemClock.UtcNow;
                await workItem.AbandonAsync();
                Assert.AreEqual(1, (await queue.GetQueueStatsAsync()).Abandoned);

                workItem = await queue.DequeueAsync(TimeSpan.FromSeconds(5));
                var elapsed = SystemClock.UtcNow.Subtract(startTime);
                _logger.DebugFormat("Time {0}", elapsed);
                Assert.NotNull(workItem);
                Assert.True(elapsed > TimeSpan.FromSeconds(.95));
                await workItem.CompleteAsync();
                Assert.AreEqual(0, (await queue.GetQueueStatsAsync()).Queued);
            }
            finally
            {
                await CleanupQueueAsync(queue);
            }
        }

        public virtual async Task CanRunWorkItemWithMetricsAsync()
        {
            int completedCount = 0;

            using (var metrics = new InMemoryMetricsClient(new InMemoryMetricsClientOptions { Buffered = false, LoggerFactory = LogFactory }))
            {
                var behavior = new MetricsQueueBehavior<WorkItemData>(metrics, "metric", TimeSpan.FromMilliseconds(100), loggerFactory: LogFactory);
                var options = new InMemoryQueueOptions<WorkItemData> { Behaviors = new[] { behavior }, LoggerFactory = LogFactory };
                using (var queue = new InMemoryQueue<WorkItemData>(options))
                {
                    Func<object, CompletedEventArgs<WorkItemData>, Task> handler = (sender, e) =>
                    {
                        completedCount++;
                        return Task.CompletedTask;
                    };

                    using (queue.Completed.AddHandler(handler))
                    {
                        _logger.Debug("Before enqueue");
                        await queue.EnqueueAsync(new SimpleWorkItem { Id = 1, Data = "Testing" });
                        await queue.EnqueueAsync(new SimpleWorkItem { Id = 2, Data = "Testing" });
                        await queue.EnqueueAsync(new SimpleWorkItem { Id = 3, Data = "Testing" });

                        await SystemClock.SleepAsync(100);

                        _logger.Debug("Before dequeue");
                        var item = await queue.DequeueAsync();
                        await item.CompleteAsync();

                        item = await queue.DequeueAsync();
                        await item.CompleteAsync();

                        item = await queue.DequeueAsync();
                        await item.AbandonAsync();

                        _logger.Debug("Before asserts");
                        Assert.AreEqual(2, completedCount);

                        await SystemClock.SleepAsync(100); // flush metrics queue behaviors
                        await metrics.FlushAsync();
                        XUnitAssert.InRange((await metrics.GetGaugeStatsAsync("metric.workitemdata.count")).Max, 1, 3);
                        XUnitAssert.InRange((await metrics.GetGaugeStatsAsync("metric.workitemdata.working")).Max, 0, 1);

                        Assert.AreEqual(3, await metrics.GetCounterCountAsync("metric.workitemdata.simple.enqueued"));
                        Assert.AreEqual(3, await metrics.GetCounterCountAsync("metric.workitemdata.enqueued"));

                        Assert.AreEqual(3, await metrics.GetCounterCountAsync("metric.workitemdata.simple.dequeued"));
                        Assert.AreEqual(3, await metrics.GetCounterCountAsync("metric.workitemdata.dequeued"));

                        Assert.AreEqual(2, await metrics.GetCounterCountAsync("metric.workitemdata.simple.completed"));
                        Assert.AreEqual(2, await metrics.GetCounterCountAsync("metric.workitemdata.completed"));

                        Assert.AreEqual(1, await metrics.GetCounterCountAsync("metric.workitemdata.simple.abandoned"));
                        Assert.AreEqual(1, await metrics.GetCounterCountAsync("metric.workitemdata.abandoned"));

                        var queueTiming = await metrics.GetTimerStatsAsync("metric.workitemdata.simple.queuetime");
                        Assert.AreEqual(3, queueTiming.Count);
                        queueTiming = await metrics.GetTimerStatsAsync("metric.workitemdata.queuetime");
                        Assert.AreEqual(3, queueTiming.Count);

                        var processTiming = await metrics.GetTimerStatsAsync("metric.workitemdata.simple.processtime");
                        Assert.AreEqual(3, processTiming.Count);
                        processTiming = await metrics.GetTimerStatsAsync("metric.workitemdata.processtime");
                        Assert.AreEqual(3, processTiming.Count);

                        var queueStats = await metrics.GetQueueStatsAsync("metric.workitemdata");
                        Assert.AreEqual(3, queueStats.Enqueued.Count);
                        Assert.AreEqual(3, queueStats.Dequeued.Count);
                        Assert.AreEqual(2, queueStats.Completed.Count);
                        Assert.AreEqual(1, queueStats.Abandoned.Count);
                        XUnitAssert.InRange(queueStats.Count.Max, 1, 3);
                        XUnitAssert.InRange(queueStats.Working.Max, 0, 1);

                        var subQueueStats = await metrics.GetQueueStatsAsync("metric.workitemdata", "simple");
                        Assert.AreEqual(3, subQueueStats.Enqueued.Count);
                        Assert.AreEqual(3, subQueueStats.Dequeued.Count);
                        Assert.AreEqual(2, subQueueStats.Completed.Count);
                        Assert.AreEqual(1, subQueueStats.Abandoned.Count);
                    }
                }
            }
        }

        public virtual async Task CanRenewLockAsync()
        {
            // Need large value to reproduce this test
            var workItemTimeout = TimeSpan.FromSeconds(1);
            // Slightly shorter than the timeout to ensure we haven't lost the lock
            var renewWait = TimeSpan.FromSeconds(workItemTimeout.TotalSeconds * .25d);

            var queue = GetQueue(retryDelay: TimeSpan.Zero, workItemTimeout: workItemTimeout);
            if (queue == null)
                return;

            try
            {
                await queue.DeleteQueueAsync();
                await AssertEmptyQueueAsync(queue);

                await queue.EnqueueAsync(new SimpleWorkItem
                {
                    Data = "Hello"
                });
                var entry = await queue.DequeueAsync(TimeSpan.Zero);
                Assert.NotNull(entry);
                Assert.AreEqual("Hello", entry.Value.Data);

                _logger.Debug($"Waiting for {renewWait} before renewing lock");
                await SystemClock.SleepAsync(renewWait);
                _logger.Debug($"Renewing lock");
                await entry.RenewLockAsync();
                _logger.Debug($"Waiting for {renewWait} to see if lock was renewed");
                await SystemClock.SleepAsync(renewWait);

                // We shouldn't get another item here if RenewLock works.
                _logger.Debug($"Attempting to dequeue item that shouldn't exist");
                var nullWorkItem = await queue.DequeueAsync(TimeSpan.Zero);
                Assert.Null(nullWorkItem);
                await entry.CompleteAsync();
                Assert.AreEqual(0, (await queue.GetQueueStatsAsync()).Queued);
            }
            finally
            {
                await CleanupQueueAsync(queue);
            }
        }

        public virtual async Task CanAbandonQueueEntryOnceAsync()
        {
            var queue = GetQueue();
            if (queue == null)
                return;

            try
            {
                await queue.DeleteQueueAsync();
                await AssertEmptyQueueAsync(queue);

                await queue.EnqueueAsync(new SimpleWorkItem { Data = "Hello" });
                Assert.AreEqual(1, (await queue.GetQueueStatsAsync()).Enqueued);

                var workItem = await queue.DequeueAsync(TimeSpan.Zero);
                Assert.NotNull(workItem);
                Assert.AreEqual("Hello", workItem.Value.Data);
                Assert.AreEqual(1, (await queue.GetQueueStatsAsync()).Dequeued);

                await workItem.AbandonAsync();
                Assert.True(workItem.IsAbandoned);
                Assert.False(workItem.IsCompleted);
                XUnitAssert.ThrowsAnyAsync<Exception>(async() =>await workItem.AbandonAsync());
                XUnitAssert.ThrowsAnyAsync<Exception>(async() => await workItem.CompleteAsync());
                XUnitAssert.ThrowsAnyAsync<Exception>(async() => await workItem.CompleteAsync());

                var stats = await queue.GetQueueStatsAsync();
                Assert.AreEqual(1, stats.Abandoned);
                Assert.AreEqual(0, stats.Completed);
                Assert.AreEqual(0, stats.Deadletter);
                Assert.AreEqual(1, stats.Dequeued);
                Assert.AreEqual(1, stats.Enqueued);
                Assert.AreEqual(0, stats.Errors);
                XUnitAssert.InRange(stats.Queued, 0, 1);
                Assert.AreEqual(0, stats.Timeouts);
                Assert.AreEqual(0, stats.Working);

                if (workItem is QueueEntry<SimpleWorkItem> queueEntry)
                    Assert.AreEqual(1, queueEntry.Attempts);

                await queue.EnqueueAsync(new SimpleWorkItem { Data = "Hello" });
                workItem = await queue.DequeueAsync(TimeSpan.Zero);

                await queue.AbandonAsync(workItem);
                Assert.True(workItem.IsAbandoned);
                Assert.False(workItem.IsCompleted);
                XUnitAssert.ThrowsAnyAsync<Exception>(async () => await workItem.CompleteAsync());
                XUnitAssert.ThrowsAnyAsync<Exception>(async () => await workItem.AbandonAsync());
                XUnitAssert.ThrowsAnyAsync<Exception>(async () => await queue.AbandonAsync(workItem));
                XUnitAssert.ThrowsAnyAsync<Exception>(async () => await queue.CompleteAsync(workItem));
            }
            finally
            {
                await CleanupQueueAsync(queue);
            }
        }

        public virtual async Task CanCompleteQueueEntryOnceAsync()
        {
            var queue = GetQueue();
            if (queue == null)
                return;

            try
            {
                await queue.DeleteQueueAsync();
                await queue.EnqueueAsync(new SimpleWorkItem { Data = "Hello" });

                Assert.AreEqual(1, (await queue.GetQueueStatsAsync()).Enqueued);

                var workItem = await queue.DequeueAsync(TimeSpan.Zero);
                Assert.NotNull(workItem);
                Assert.AreEqual("Hello", workItem.Value.Data);
                Assert.AreEqual(1, (await queue.GetQueueStatsAsync()).Dequeued);

                await workItem.CompleteAsync();
                Assert.ThrowsAsync<InvalidOperationException>(() => workItem.CompleteAsync());
                Assert.ThrowsAsync<InvalidOperationException>(() => workItem.AbandonAsync());
                Assert.ThrowsAsync<InvalidOperationException>(() => workItem.AbandonAsync());
                var stats = await queue.GetQueueStatsAsync();
                Assert.AreEqual(0, stats.Abandoned);
                Assert.AreEqual(1, stats.Completed);
                Assert.AreEqual(0, stats.Deadletter);
                Assert.AreEqual(1, stats.Dequeued);
                Assert.AreEqual(1, stats.Enqueued);
                Assert.AreEqual(0, stats.Errors);
                Assert.AreEqual(0, stats.Queued);
                Assert.AreEqual(0, stats.Timeouts);
                Assert.AreEqual(0, stats.Working);

                if (workItem is QueueEntry<SimpleWorkItem> queueEntry)
                    Assert.AreEqual(1, queueEntry.Attempts);
            }
            finally
            {
                await CleanupQueueAsync(queue);
            }
        }

        public virtual async Task CanDequeueWithLockingAsync()
        {
            using (var cache = new InMemoryCacheClient(new InMemoryCacheClientOptions { LoggerFactory = LogFactory }))
            {
                using (var messageBus = new InMemoryMessageBus(new InMemoryMessageBusOptions { LoggerFactory = LogFactory }))
                {
                    var distributedLock = new CacheLockProvider(cache, messageBus, LogFactory);
                    await CanDequeueWithLockingImpAsync(distributedLock);
                }
            }
        }

        protected async Task CanDequeueWithLockingImpAsync(CacheLockProvider distributedLock)
        {
            var queue = GetQueue(retryDelay: TimeSpan.Zero, retries: 0);
            if (queue == null)
                return;

            try
            {
                await queue.DeleteQueueAsync();
                await AssertEmptyQueueAsync(queue);

                using (var metrics = new InMemoryMetricsClient(new InMemoryMetricsClientOptions { Buffered = false, LoggerFactory = LogFactory }))
                {
                    queue.AttachBehavior(new MetricsQueueBehavior<SimpleWorkItem>(metrics, loggerFactory: LogFactory));

                    var resetEvent = new AsyncAutoResetEvent();
                    await queue.StartWorkingAsync(async w =>
                    {
                        _logger.Info("Acquiring distributed lock in work item");
                        var l = await distributedLock.AcquireAsync("test");
                        Assert.NotNull(l);
                        _logger.Info("Acquired distributed lock");
                        SystemClock.Sleep(TimeSpan.FromMilliseconds(250));
                        await l.ReleaseAsync();
                        _logger.Info("Released distributed lock");

                        await w.CompleteAsync();
                        resetEvent.Set();
                    });

                    await queue.EnqueueAsync(new SimpleWorkItem { Data = "Hello" });
                    await resetEvent.WaitAsync(TimeSpan.FromSeconds(5).ToCancellationToken());

                    await SystemClock.SleepAsync(1);
                    var stats = await queue.GetQueueStatsAsync();
                    _logger.Info($"Completed: {stats.Completed} Errors: { stats.Errors} Deadletter: {stats.Deadletter} Working: {stats.Working} ");
                    Assert.AreEqual(1, stats.Completed);
                }
            }
            finally
            {
                await CleanupQueueAsync(queue);
            }
        }

        public virtual async Task CanHaveMultipleQueueInstancesWithLockingAsync()
        {
            using (var cache = new InMemoryCacheClient(new InMemoryCacheClientOptions { LoggerFactory = LogFactory }))
            {
                using (var messageBus = new InMemoryMessageBus(new InMemoryMessageBusOptions { LoggerFactory = LogFactory }))
                {
                    var distributedLock = new CacheLockProvider(cache, messageBus, LogFactory);
                    await CanHaveMultipleQueueInstancesWithLockingImplAsync(distributedLock);
                }
            }
        }

        protected async Task CanHaveMultipleQueueInstancesWithLockingImplAsync(CacheLockProvider distributedLock)
        {
            var queue = GetQueue(retries: 0, retryDelay: TimeSpan.Zero);
            if (queue == null)
                return;

            try
            {
                await queue.DeleteQueueAsync();
                await AssertEmptyQueueAsync(queue);

                const int workItemCount = 16;
                const int workerCount = 4;
                var countdown = new AsyncCountdownEvent(workItemCount);
                var info = new WorkInfo();
                var workers = new List<IQueue<SimpleWorkItem>> { queue };

                try
                {
                    for (int i = 0; i < workerCount; i++)
                    {
                        var q = GetQueue(retries: 0, retryDelay: TimeSpan.Zero);
                        int instanceCount = i;
                        await q.StartWorkingAsync(async w =>
                        {
                            _logger.InfoFormat("[{0}] Acquiring distributed lock in work item: {1}", instanceCount, w.Id);
                            var l = await distributedLock.AcquireAsync("test");
                            Assert.NotNull(l);
                            _logger.InfoFormat("[{0}] Acquired distributed lock: {1}", instanceCount, w.Id);
                            SystemClock.Sleep(TimeSpan.FromMilliseconds(50));
                            await l.ReleaseAsync();
                            _logger.InfoFormat("[{0}] Released distributed lock: {1}", instanceCount, w.Id);

                            await w.CompleteAsync();
                            info.IncrementCompletedCount();
                            countdown.Signal();
                            _logger.InfoFormat("[{0}] Signaled countdown: {1}", instanceCount, w.Id);
                        });
                        workers.Add(q);
                    }

                    await Run.InParallelAsync(workItemCount, async i =>
                    {
                        string id = await queue.EnqueueAsync(new SimpleWorkItem
                        {
                            Data = "Hello",
                            Id = i
                        });
                        _logger.DebugFormat("Enqueued Index: {0} Id: {1}", i, id);
                    });

                    await countdown.WaitAsync();
                    await SystemClock.SleepAsync(50);
                    _logger.DebugFormat("Completed: {0} Abandoned: {1} Error: {2}", info.CompletedCount, info.AbandonCount, info.ErrorCount);

                    _logger.Info($"Work Info Stats: Completed: { info.CompletedCount} Abandoned: {info.AbandonCount} Error: {info.ErrorCount}");
                    Assert.AreEqual(workItemCount, info.CompletedCount + info.AbandonCount + info.ErrorCount);

                    // In memory queue doesn't share state.
                    if (queue.GetType() == typeof(InMemoryQueue<SimpleWorkItem>))
                    {
                        var stats = await queue.GetQueueStatsAsync();
                        Assert.AreEqual(info.CompletedCount, stats.Completed);
                    }
                    else
                    {
                        var workerStats = new List<QueueStats>();
                        for (int i = 0; i < workers.Count; i++)
                        {
                            var stats = await workers[i].GetQueueStatsAsync();
                            _logger.Info($"Worker#{i} Working: { stats.Working} Completed: {stats.Completed} Abandoned: { stats.Abandoned} Error: {stats.Errors} Deadletter: {stats.Deadletter}");
                            workerStats.Add(stats);
                        }

                        Assert.AreEqual(info.CompletedCount, workerStats.Sum(s => s.Completed));
                    }
                }
                finally
                {
                    foreach (var q in workers)
                        await CleanupQueueAsync(q);
                }
            }
            finally
            {
                await CleanupQueueAsync(queue);
            }
        }

        protected async Task DoWorkAsync(IQueueEntry<SimpleWorkItem> w, AsyncCountdownEvent countdown, WorkInfo info)
        {
            _logger.Debug($"Starting: {w.Value.Id}");
            Assert.AreEqual("Hello", w.Value.Data);

            try
            {
                // randomly complete, abandon or blowup.
                if (RandomData.GetBool())
                {
                    _logger.Debug($"Completing: {w.Value.Id}");
                    await w.CompleteAsync();
                    info.IncrementCompletedCount();
                }
                else if (RandomData.GetBool())
                {
                    _logger.Debug($"Abandoning: {w.Value.Id}");
                    await w.AbandonAsync();
                    info.IncrementAbandonCount();
                }
                else
                {
                    _logger.Debug($"Erroring: {w.Value.Id}");
                    info.IncrementErrorCount();
                    throw new Exception();
                }
            }
            finally
            {
                _logger.Debug($"Signal {countdown.CurrentCount}");
                countdown.Signal();
            }
        }

        private async Task AssertEmptyQueueAsync(IQueue<SimpleWorkItem> queue)
        {
            var stats = await queue.GetQueueStatsAsync();
            Assert.AreEqual(0, stats.Abandoned);
            Assert.AreEqual(0, stats.Completed);
            Assert.AreEqual(0, stats.Deadletter);
            Assert.AreEqual(0, stats.Dequeued);
            Assert.AreEqual(0, stats.Enqueued);
            Assert.AreEqual(0, stats.Errors);
            Assert.AreEqual(0, stats.Queued);
            Assert.AreEqual(0, stats.Timeouts);
            Assert.AreEqual(0, stats.Working);
        }

        public virtual async Task MaintainJobNotAbandon_NotWorkTimeOutEntry()
        {
            var queue = GetQueue(retries: 0, workItemTimeout: TimeSpan.FromMilliseconds(100), retryDelay: TimeSpan.Zero);
            if (queue == null)
                return;
            try
            {
                await queue.DeleteQueueAsync();
                await AssertEmptyQueueAsync(queue);
                queue.EnqueueAsync(new SimpleWorkItem
                {
                    Data = "Hello World",
                    Id = 1
                });
                queue.EnqueueAsync(new SimpleWorkItem
                {
                    Data = "Hello World",
                    Id = 2
                });

                var dequeuedQueueItem =(await queue.DequeueAsync());
                Assert.NotNull(dequeuedQueueItem.Value);
                // The first dequeued item works for 60 milliseconds less than work timeout(100 milliseconds).
                await SystemClock.SleepAsync(60);
                await dequeuedQueueItem.CompleteAsync();
                Assert.True(dequeuedQueueItem.IsCompleted);
                Assert.False(dequeuedQueueItem.IsAbandoned);

                dequeuedQueueItem = (await queue.DequeueAsync());
                Assert.NotNull(dequeuedQueueItem.Value);
                // The second dequeued item works for 60 milliseconds less than work timeout(100 milliseconds).
                await SystemClock.SleepAsync(60);
                await dequeuedQueueItem.CompleteAsync();
                Assert.True(dequeuedQueueItem.IsCompleted);
                Assert.False(dequeuedQueueItem.IsAbandoned);

                var stats = await queue.GetQueueStatsAsync();
                Assert.AreEqual(0, stats.Working);
                Assert.AreEqual(0, stats.Abandoned);
                Assert.AreEqual(2, stats.Completed);
            }
            finally
            {
                await CleanupQueueAsync(queue);
            }
        }

        public virtual void Dispose()
        {
            var queue = GetQueue();
            if (queue == null)
                return;

            using (queue)
            {
                queue.DeleteQueueAsync().GetAwaiter().GetResult();
            }
        }
    }

    public class WorkInfo
    {
        private int _abandonCount = 0;
        private int _errorCount = 0;
        private int _completedCount = 0;

        public int AbandonCount => _abandonCount;
        public int ErrorCount => _errorCount;
        public int CompletedCount => _completedCount;

        public void IncrementAbandonCount()
        {
            Interlocked.Increment(ref _abandonCount);
        }

        public void IncrementErrorCount()
        {
            Interlocked.Increment(ref _errorCount);
        }

        public void IncrementCompletedCount()
        {
            Interlocked.Increment(ref _completedCount);
        }
    }
}