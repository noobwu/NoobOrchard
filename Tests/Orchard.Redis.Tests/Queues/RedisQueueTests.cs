using Nito.AsyncEx;
using NUnit.Framework;
using Orchard.Metrics;
using Orchard.Queues;
using Orchard.Redis.Caching;
using Orchard.Redis.Messaging;
using Orchard.Redis.Queues;
using Orchard.Tests;
using Orchard.Tests.Queue;
using Orchard.Threading.Locking;
using Orchard.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#pragma warning disable 4014

namespace Orchard.Redis.Tests.Queues
{
    public class RedisQueueTests : QueueTestBase {
        public RedisQueueTests(){
            var muxer = SharedConnection.GetMuxer();
            while (muxer.CountAllKeysAsync().GetAwaiter().GetResult() != 0)
                muxer.FlushAllAsync().GetAwaiter().GetResult();
        }

        protected override IQueue<SimpleWorkItem> GetQueue(int retries = 1, TimeSpan? workItemTimeout = null, TimeSpan? retryDelay = null, int deadLetterMaxItems = 100, bool runQueueMaintenance = true) {
            var queue = new RedisQueue<SimpleWorkItem>(new RedisQueueOptions<SimpleWorkItem> {
                ConnectionMultiplexer = SharedConnection.GetMuxer(),
                Retries = retries,
                RetryDelay = retryDelay.GetValueOrDefault(TimeSpan.FromMinutes(1)),
                DeadLetterMaxItems = deadLetterMaxItems,
                WorkItemTimeout = workItemTimeout.GetValueOrDefault(TimeSpan.FromMinutes(5)),
                RunMaintenanceTasks = runQueueMaintenance,
                LoggerFactory = LogFactory
            });

            _logger.Debug($"Queue Id: {queue.QueueId}");
            return queue;
        }

        [Test]
        public override Task CanQueueAndDequeueWorkItemAsync() {
            return base.CanQueueAndDequeueWorkItemAsync();
        }

        [Test]
        public override Task CanDequeueWithCancelledTokenAsync() {
            return base.CanDequeueWithCancelledTokenAsync();
        }

        [Test]
        public override Task CanDequeueEfficientlyAsync() {
            return base.CanDequeueEfficientlyAsync();
        }

        [Test]
        public override Task CanResumeDequeueEfficientlyAsync() {
            return base.CanResumeDequeueEfficientlyAsync();
        }

        [Test]
        public override Task CanQueueAndDequeueMultipleWorkItemsAsync() {
            return base.CanQueueAndDequeueMultipleWorkItemsAsync();
        }

        [Test]
        public override Task WillNotWaitForItemAsync() {
            return base.WillNotWaitForItemAsync();
        }

        [Test]
        public override Task WillWaitForItemAsync() {
            return base.WillWaitForItemAsync();
        }

        [Test]
        public override Task DequeueWaitWillGetSignaledAsync() {
            return base.DequeueWaitWillGetSignaledAsync();
        }

        [Test]
        public override Task CanUseQueueWorkerAsync() {
            return base.CanUseQueueWorkerAsync();
        }

        [Test]
        public override Task CanRenewLockAsync() {
            return base.CanRenewLockAsync();
        }

        [Test]
        public override Task CanHandleErrorInWorkerAsync() {
            return base.CanHandleErrorInWorkerAsync();
        }

        [Test]
        public override Task WorkItemsWillTimeoutAsync() {
            return base.WorkItemsWillTimeoutAsync();
        }

        [Test]
        public override Task WorkItemsWillGetMovedToDeadletterAsync() {
            return base.WorkItemsWillGetMovedToDeadletterAsync();
        }

        [Test]
        public override Task CanAutoCompleteWorkerAsync() {
            return base.CanAutoCompleteWorkerAsync();
        }

        [Test]
        public override Task CanHaveMultipleQueueInstancesAsync() {
            return base.CanHaveMultipleQueueInstancesAsync();
        }

        [Test]
        public override Task CanDelayRetryAsync() {
            return base.CanDelayRetryAsync();
        }

        [Test]
        public override Task CanRunWorkItemWithMetricsAsync() {
            return base.CanRunWorkItemWithMetricsAsync();
        }

        [Test]
        public override Task CanAbandonQueueEntryOnceAsync() {
            return base.CanAbandonQueueEntryOnceAsync();
        }

        [Test]
        public override Task CanCompleteQueueEntryOnceAsync() {
            return base.CanCompleteQueueEntryOnceAsync();
        }

        [Test]
        public override async Task CanDequeueWithLockingAsync() {
            var muxer = SharedConnection.GetMuxer();
            using (var cache = new RedisCacheClient(new RedisCacheClientOptions { ConnectionMultiplexer = muxer, LoggerFactory = LogFactory })) {
                using (var messageBus = new RedisMessageBus(new RedisMessageBusOptions { Subscriber = muxer.GetSubscriber(), Topic = "test-queue", LoggerFactory = LogFactory })) {
                    var distributedLock = new CacheLockProvider(cache, messageBus, LogFactory);
                    await CanDequeueWithLockingImpAsync(distributedLock);
                }
            }
        }

        [Test]
        public override async Task CanHaveMultipleQueueInstancesWithLockingAsync() {
            var muxer = SharedConnection.GetMuxer();
            using (var cache = new RedisCacheClient(new RedisCacheClientOptions { ConnectionMultiplexer = muxer, LoggerFactory = LogFactory })) {
                using (var messageBus = new RedisMessageBus(new RedisMessageBusOptions { Subscriber = muxer.GetSubscriber(), Topic = "test-queue", LoggerFactory = LogFactory })) {
                    var distributedLock = new CacheLockProvider(cache, messageBus, LogFactory);
                    await CanHaveMultipleQueueInstancesWithLockingImplAsync(distributedLock);
                }
            }
        }

        [Test]
        public async Task VerifyCacheKeysAreCorrect() {
            var queue = GetQueue(retries: 3, workItemTimeout: TimeSpan.FromSeconds(2), retryDelay: TimeSpan.Zero, runQueueMaintenance: false);
            if (queue == null)
                return;

            using (queue) {
                var muxer = SharedConnection.GetMuxer();
                var db = muxer.GetDatabase();

                string id = await queue.EnqueueAsync(new SimpleWorkItem { Data = "blah", Id = 1 });
                Assert.True(await db.KeyExistsAsync("q:SimpleWorkItem:" + id));
                Assert.AreEqual(1, await db.ListLengthAsync("q:SimpleWorkItem:in"));
                Assert.True(await db.KeyExistsAsync("q:SimpleWorkItem:" + id + ":enqueued"));
                Assert.AreEqual(3, await muxer.CountAllKeysAsync());

                _logger.Info("-----");

                var workItem = await queue.DequeueAsync();
                Assert.True(await db.KeyExistsAsync("q:SimpleWorkItem:" + id));
                Assert.AreEqual(0, await db.ListLengthAsync("q:SimpleWorkItem:in"));
                Assert.AreEqual(1, await db.ListLengthAsync("q:SimpleWorkItem:work"));
                Assert.True(await db.KeyExistsAsync("q:SimpleWorkItem:" + id + ":enqueued"));
                Assert.True(await db.KeyExistsAsync("q:SimpleWorkItem:" + id + ":renewed"));
                Assert.True(await db.KeyExistsAsync("q:SimpleWorkItem:" + id + ":dequeued"));
                Assert.AreEqual(5, await muxer.CountAllKeysAsync());

                _logger.Info("-----");

                await workItem.CompleteAsync();
                Assert.False(await db.KeyExistsAsync("q:SimpleWorkItem:" + id));
                Assert.False(await db.KeyExistsAsync("q:SimpleWorkItem:" + id + ":enqueued"));
                Assert.False(await db.KeyExistsAsync("q:SimpleWorkItem:" + id + ":renewed"));
                Assert.False(await db.KeyExistsAsync("q:SimpleWorkItem:" + id + ":dequeued"));
                Assert.AreEqual(0, await db.ListLengthAsync("q:SimpleWorkItem:in"));
                Assert.AreEqual(0, await db.ListLengthAsync("q:SimpleWorkItem:work"));
                Assert.AreEqual(0, await muxer.CountAllKeysAsync());
            }
        }

        [Test]
        public async Task VerifyCacheKeysAreCorrectAfterAbandon() {
            var queue = GetQueue(retries: 2, workItemTimeout: TimeSpan.FromMilliseconds(100), retryDelay: TimeSpan.Zero, runQueueMaintenance: false) as RedisQueue<SimpleWorkItem>;
            if (queue == null)
                return;

            using (TestSystemClock.Install()) {
                using (queue) {
                    var muxer = SharedConnection.GetMuxer();
                    var db = muxer.GetDatabase();

                    string id = await queue.EnqueueAsync(new SimpleWorkItem {
                        Data = "blah",
                        Id = 1
                    });
                    _logger.DebugFormat("SimpleWorkItem Id: {0}", id);

                    var workItem = await queue.DequeueAsync();
                    await workItem.AbandonAsync();
                    Assert.True(await db.KeyExistsAsync("q:SimpleWorkItem:" + id));
                    Assert.AreEqual(1, await db.ListLengthAsync("q:SimpleWorkItem:in"));
                    Assert.AreEqual(0, await db.ListLengthAsync("q:SimpleWorkItem:work"));
                    Assert.False(await db.KeyExistsAsync("q:SimpleWorkItem:" + id + ":dequeued"));
                    Assert.True(await db.KeyExistsAsync("q:SimpleWorkItem:" + id + ":enqueued"));
                    Assert.False(await db.KeyExistsAsync("q:SimpleWorkItem:" + id + ":renewed"));
                    Assert.AreEqual(1, await db.StringGetAsync("q:SimpleWorkItem:" + id + ":attempts"));
                    Assert.AreEqual(4, await muxer.CountAllKeysAsync());

                    workItem = await queue.DequeueAsync();
                    Assert.True(await db.KeyExistsAsync("q:SimpleWorkItem:" + id));
                    Assert.AreEqual(0, await db.ListLengthAsync("q:SimpleWorkItem:in"));
                    Assert.AreEqual(1, await db.ListLengthAsync("q:SimpleWorkItem:work"));
                    Assert.True(await db.KeyExistsAsync("q:SimpleWorkItem:" + id + ":dequeued"));
                    Assert.True(await db.KeyExistsAsync("q:SimpleWorkItem:" + id + ":enqueued"));
                    Assert.True(await db.KeyExistsAsync("q:SimpleWorkItem:" + id + ":renewed"));
                    Assert.AreEqual(1, await db.StringGetAsync("q:SimpleWorkItem:" + id + ":attempts"));
                    Assert.AreEqual(6, await muxer.CountAllKeysAsync());

                    // let the work item timeout and become auto abandoned.
                    SystemClock.Test.AddTime(TimeSpan.FromMilliseconds(250));
                    await queue.DoMaintenanceWorkAsync();
                    Assert.True(await db.KeyExistsAsync("q:SimpleWorkItem:" + id));
                    Assert.AreEqual(1, await db.ListLengthAsync("q:SimpleWorkItem:in"));
                    Assert.AreEqual(0, await db.ListLengthAsync("q:SimpleWorkItem:work"));
                    Assert.False(await db.KeyExistsAsync("q:SimpleWorkItem:" + id + ":dequeued"));
                    Assert.True(await db.KeyExistsAsync("q:SimpleWorkItem:" + id + ":enqueued"));
                    Assert.False(await db.KeyExistsAsync("q:SimpleWorkItem:" + id + ":renewed"));
                    Assert.AreEqual(2, await db.StringGetAsync("q:SimpleWorkItem:" + id + ":attempts"));
                    Assert.AreEqual(1, (await queue.GetQueueStatsAsync()).Timeouts);
                    XUnitAssert.InRange(await muxer.CountAllKeysAsync(), 3, 4);

                    // should go to deadletter now
                    workItem = await queue.DequeueAsync();
                    await workItem.AbandonAsync();
                    Assert.True(await db.KeyExistsAsync("q:SimpleWorkItem:" + id));
                    Assert.AreEqual(0, await db.ListLengthAsync("q:SimpleWorkItem:in"));
                    Assert.AreEqual(0, await db.ListLengthAsync("q:SimpleWorkItem:work"));
                    Assert.AreEqual(1, await db.ListLengthAsync("q:SimpleWorkItem:dead"));
                    Assert.False(await db.KeyExistsAsync("q:SimpleWorkItem:" + id + ":dequeued"));
                    Assert.True(await db.KeyExistsAsync("q:SimpleWorkItem:" + id + ":enqueued"));
                    Assert.False(await db.KeyExistsAsync("q:SimpleWorkItem:" + id + ":renewed"));
                    Assert.AreEqual(3, await db.StringGetAsync("q:SimpleWorkItem:" + id + ":attempts"));
                    XUnitAssert.InRange(await muxer.CountAllKeysAsync(), 4, 5);
                }
            }
        }

        [Test]
        public async Task VerifyCacheKeysAreCorrectAfterAbandonWithRetryDelay() {
            var queue = GetQueue(retries: 2, workItemTimeout: TimeSpan.FromMilliseconds(100), retryDelay: TimeSpan.FromMilliseconds(250), runQueueMaintenance: false) as RedisQueue<SimpleWorkItem>;
            if (queue == null)
                return;

            using (TestSystemClock.Install()) {
                using (queue) {
                    var muxer = SharedConnection.GetMuxer();
                    var db = muxer.GetDatabase();

                    string id = await queue.EnqueueAsync(new SimpleWorkItem {
                        Data = "blah",
                        Id = 1
                    });
                    var workItem = await queue.DequeueAsync();
                    await workItem.AbandonAsync();
                    Assert.True(await db.KeyExistsAsync("q:SimpleWorkItem:" + id));
                    Assert.AreEqual(0, await db.ListLengthAsync("q:SimpleWorkItem:in"));
                    Assert.AreEqual(0, await db.ListLengthAsync("q:SimpleWorkItem:work"));
                    Assert.AreEqual(1, await db.ListLengthAsync("q:SimpleWorkItem:wait"));
                    Assert.False(await db.KeyExistsAsync("q:SimpleWorkItem:" + id + ":dequeued"));
                    Assert.True(await db.KeyExistsAsync("q:SimpleWorkItem:" + id + ":enqueued"));
                    Assert.False(await db.KeyExistsAsync("q:SimpleWorkItem:" + id + ":renewed"));
                    Assert.AreEqual(1, await db.StringGetAsync("q:SimpleWorkItem:" + id + ":attempts"));
                    Assert.True(await db.KeyExistsAsync("q:SimpleWorkItem:" + id + ":wait"));
                    Assert.AreEqual(5, await muxer.CountAllKeysAsync());

                    SystemClock.Test.AddTime(TimeSpan.FromSeconds(1));
                    await queue.DoMaintenanceWorkAsync();
                    Assert.True(await db.KeyExistsAsync("q:SimpleWorkItem:" + id));
                    Assert.AreEqual(1, await db.ListLengthAsync("q:SimpleWorkItem:in"));
                    Assert.AreEqual(0, await db.ListLengthAsync("q:SimpleWorkItem:work"));
                    Assert.AreEqual(0, await db.ListLengthAsync("q:SimpleWorkItem:wait"));
                    Assert.False(await db.KeyExistsAsync("q:SimpleWorkItem:" + id + ":dequeued"));
                    Assert.True(await db.KeyExistsAsync("q:SimpleWorkItem:" + id + ":enqueued"));
                    Assert.False(await db.KeyExistsAsync("q:SimpleWorkItem:" + id + ":renewed"));
                    Assert.AreEqual(1, await db.StringGetAsync("q:SimpleWorkItem:" + id + ":attempts"));
                    Assert.False(await db.KeyExistsAsync("q:SimpleWorkItem:" + id + ":wait"));
                    XUnitAssert.InRange(await muxer.CountAllKeysAsync(), 4, 5);

                    workItem = await queue.DequeueAsync();
                    Assert.True(await db.KeyExistsAsync("q:SimpleWorkItem:" + id));
                    Assert.AreEqual(0, await db.ListLengthAsync("q:SimpleWorkItem:in"));
                    Assert.AreEqual(1, await db.ListLengthAsync("q:SimpleWorkItem:work"));
                    Assert.True(await db.KeyExistsAsync("q:SimpleWorkItem:" + id + ":dequeued"));
                    Assert.True(await db.KeyExistsAsync("q:SimpleWorkItem:" + id + ":enqueued"));
                    Assert.True(await db.KeyExistsAsync("q:SimpleWorkItem:" + id + ":renewed"));
                    Assert.AreEqual(1, await db.StringGetAsync("q:SimpleWorkItem:" + id + ":attempts"));
                    XUnitAssert.InRange(await muxer.CountAllKeysAsync(), 6, 7);

                    await workItem.CompleteAsync();
                    Assert.False(await db.KeyExistsAsync("q:SimpleWorkItem:" + id));
                    Assert.False(await db.KeyExistsAsync("q:SimpleWorkItem:" + id + ":enqueued"));
                    Assert.False(await db.KeyExistsAsync("q:SimpleWorkItem:" + id + ":dequeued"));
                    Assert.AreEqual(0, await db.ListLengthAsync("q:SimpleWorkItem:in"));
                    Assert.AreEqual(0, await db.ListLengthAsync("q:SimpleWorkItem:work"));
                    XUnitAssert.InRange(await muxer.CountAllKeysAsync(), 0, 1);
                }
            }
        }

        [Test]
        public async Task CanTrimDeadletterItems() {
            var queue = GetQueue(retries: 0, workItemTimeout: TimeSpan.FromMilliseconds(50), deadLetterMaxItems: 3, runQueueMaintenance: false) as RedisQueue<SimpleWorkItem>;
            if (queue == null)
                return;

            using (queue) {
                var muxer = SharedConnection.GetMuxer();
                var db = muxer.GetDatabase();

                var workItemIds = new List<string>();
                for (int i = 0; i < 10; i++) {
                    string id = await queue.EnqueueAsync(new SimpleWorkItem {Data = "blah", Id = i});
                    _logger.Debug(id);
                    workItemIds.Add(id);
                }

                for (int i = 0; i < 10; i++) {
                    var workItem = await queue.DequeueAsync();
                    await workItem.AbandonAsync();
                    _logger.Debug("Abandoning: " + workItem.Id);
                }

                workItemIds.Reverse();
                await queue.DoMaintenanceWorkAsync();

                foreach (object id in workItemIds.Take(3)) {
                    _logger.Debug("Checking: " + id);
                    Assert.True(await db.KeyExistsAsync("q:SimpleWorkItem:" + id));
                }

                Assert.AreEqual(0, await db.ListLengthAsync("q:SimpleWorkItem:in"));
                Assert.AreEqual(0, await db.ListLengthAsync("q:SimpleWorkItem:work"));
                Assert.AreEqual(0, await db.ListLengthAsync("q:SimpleWorkItem:wait"));
                Assert.AreEqual(3, await db.ListLengthAsync("q:SimpleWorkItem:dead"));
                XUnitAssert.InRange(await muxer.CountAllKeysAsync(), 10, 11);
            }
        }

        // TODO: Need to write tests that verify the cache data is correct after each operation.

        [Ignore("Performance Test")]
        public async Task MeasureThroughputWithRandomFailures() {
            var queue = GetQueue(retries: 3, workItemTimeout: TimeSpan.FromSeconds(2), retryDelay: TimeSpan.Zero);
            if (queue == null)
                return;

            using (queue) {
                await queue.DeleteQueueAsync();

                const int workItemCount = 1000;
                for (int i = 0; i < workItemCount; i++) {
                    await queue.EnqueueAsync(new SimpleWorkItem {
                        Data = "Hello"
                    });
                }
                Assert.AreEqual(workItemCount, (await queue.GetQueueStatsAsync()).Queued);

                var metrics = new InMemoryMetricsClient(new InMemoryMetricsClientOptions());
                var workItem = await queue.DequeueAsync(TimeSpan.Zero);
                while (workItem != null) {
                    Assert.AreEqual("Hello", workItem.Value.Data);
                    if (RandomData.GetBool(10))
                        await workItem.AbandonAsync();
                    else
                        await workItem.CompleteAsync();

                    await metrics.CounterAsync("work");

                    workItem = await queue.DequeueAsync(TimeSpan.FromMilliseconds(100));
                }
                _logger.Debug((await metrics.GetCounterStatsAsync("work")).ToString());

                var stats = await queue.GetQueueStatsAsync();
                Assert.True(stats.Dequeued >= workItemCount);
                Assert.AreEqual(workItemCount, stats.Completed + stats.Deadletter);
                Assert.AreEqual(0, stats.Queued);

                var muxer = SharedConnection.GetMuxer();
                _logger.DebugFormat("# Keys: {0}", muxer.CountAllKeysAsync());
            }
        }

        [Ignore("Performance Test")]
        public async Task MeasureThroughput() {
            var queue = GetQueue(retries: 3, workItemTimeout: TimeSpan.FromSeconds(2), retryDelay: TimeSpan.FromSeconds(1));
            if (queue == null)
                return;

            using (queue) {
                await queue.DeleteQueueAsync();

                const int workItemCount = 1000;
                for (int i = 0; i < workItemCount; i++) {
                    await queue.EnqueueAsync(new SimpleWorkItem {
                        Data = "Hello"
                    });
                }
                Assert.AreEqual(workItemCount, (await queue.GetQueueStatsAsync()).Queued);

                var metrics = new InMemoryMetricsClient(new InMemoryMetricsClientOptions());
                var workItem = await queue.DequeueAsync(TimeSpan.Zero);
                while (workItem != null) {
                    Assert.AreEqual("Hello", workItem.Value.Data);
                    await workItem.CompleteAsync();
                    await metrics.CounterAsync("work");

                    workItem = await queue.DequeueAsync(TimeSpan.Zero);
                }
                _logger.Debug((await metrics.GetCounterStatsAsync("work")).ToString());

                var stats = await queue.GetQueueStatsAsync();
                Assert.AreEqual(workItemCount, stats.Dequeued);
                Assert.AreEqual(workItemCount, stats.Completed);
                Assert.AreEqual(0, stats.Queued);

                var muxer = SharedConnection.GetMuxer();
                _logger.DebugFormat("# Keys: {0}", muxer.CountAllKeysAsync());
            }
        }

        [Ignore("Performance Test")]
        public async Task MeasureWorkerThroughput() {
            var queue = GetQueue(retries: 3, workItemTimeout: TimeSpan.FromSeconds(2), retryDelay: TimeSpan.FromSeconds(1));
            if (queue == null)
                return;

            using (queue) {
                await queue.DeleteQueueAsync();

                const int workItemCount = 1;
                for (int i = 0; i < workItemCount; i++) {
                    await queue.EnqueueAsync(new SimpleWorkItem {
                        Data = "Hello"
                    });
                }
                Assert.AreEqual(workItemCount, (await queue.GetQueueStatsAsync()).Queued);

                var countdown = new AsyncCountdownEvent(workItemCount);
                var metrics = new InMemoryMetricsClient(new InMemoryMetricsClientOptions());
                await queue.StartWorkingAsync(async workItem => {
                    Assert.AreEqual("Hello", workItem.Value.Data);
                    await workItem.CompleteAsync();
                    await metrics.CounterAsync("work");
                    countdown.Signal();
                });

                await countdown.WaitAsync();
                Assert.AreEqual(0, countdown.CurrentCount);
                _logger.Debug((await metrics.GetCounterStatsAsync("work")).ToString());

                var stats = await queue.GetQueueStatsAsync();
                Assert.AreEqual(workItemCount, stats.Dequeued);
                Assert.AreEqual(workItemCount, stats.Completed);
                Assert.AreEqual(0, stats.Queued);

                var muxer = SharedConnection.GetMuxer();
                _logger.DebugFormat("# Keys: {0}", muxer.CountAllKeysAsync());
            }
        }
    }
}