using NUnit.Framework;
using Orchard.Queues;
using Orchard.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Orchard.Tests.Queue
{
    public class InMemoryQueueTests : QueueTestBase {
        private IQueue<SimpleWorkItem> _queue;

        protected override IQueue<SimpleWorkItem> GetQueue(int retries = 1, TimeSpan? workItemTimeout = null, TimeSpan? retryDelay = null, int deadLetterMaxItems = 100, bool runQueueMaintenance = true) {
            if (_queue == null)
                _queue = new InMemoryQueue<SimpleWorkItem>(new InMemoryQueueOptions<SimpleWorkItem> {
                    Retries = retries,
                    RetryDelay = retryDelay.GetValueOrDefault(TimeSpan.FromMinutes(1)),
                    WorkItemTimeout = workItemTimeout.GetValueOrDefault(TimeSpan.FromMinutes(5)),
                    LoggerFactory = LogFactory
                });

            _logger.Debug($"Queue Id: { _queue.QueueId}");
            return _queue;
        }

        protected override async Task CleanupQueueAsync(IQueue<SimpleWorkItem> queue) {
            if (queue == null)
                return;

            try {
                await queue.DeleteQueueAsync();
            } catch (Exception ex) {
                _logger.Error("Error cleaning up queue",ex);
            }
        }

        [Test]
        public async Task TestAsyncEvents() {
            using (var q = new InMemoryQueue<SimpleWorkItem>(new InMemoryQueueOptions<SimpleWorkItem> { LoggerFactory = LogFactory })) {
                var disposables = new List<IDisposable>(5);
                try {
                    disposables.Add(q.Enqueuing.AddHandler(async (sender, args) => {
                        await SystemClock.SleepAsync(250);
                        _logger.Info("First Enqueuing.");
                    }));
                    disposables.Add(q.Enqueuing.AddHandler(async (sender, args) => {
                        await SystemClock.SleepAsync(250);
                        _logger.Info("Second Enqueuing.");
                    }));
                    disposables.Add(q.Enqueued.AddHandler(async (sender, args) => {
                        await SystemClock.SleepAsync(250);
                        _logger.Info("First.");
                    }));
                    disposables.Add(q.Enqueued.AddHandler(async (sender, args) => {
                        await SystemClock.SleepAsync(250);
                        _logger.Info("Second.");
                    }));

                    var sw = Stopwatch.StartNew();
                    await q.EnqueueAsync(new SimpleWorkItem());
                    sw.Stop();
                    _logger.DebugFormat("Time {0}", sw.Elapsed);

                    sw.Restart();
                    await q.EnqueueAsync(new SimpleWorkItem());
                    sw.Stop();
                    _logger.DebugFormat("Time {0}", sw.Elapsed);
                } finally {
                    foreach (var disposable in disposables)
                        disposable.Dispose();
                }
            }
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
        public override Task CanHandleErrorInWorkerAsync() {
            return base.CanHandleErrorInWorkerAsync();
        }

        [Test]
        public override Task WorkItemsWillTimeoutAsync() {
            using (TestSystemClock.Install()) {
                return base.WorkItemsWillTimeoutAsync();
            }
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
        public override Task CanRenewLockAsync() {
            return base.CanRenewLockAsync();
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
        public override Task CanDequeueWithLockingAsync() {
            return base.CanDequeueWithLockingAsync();
        }

        [Test]
        public override Task CanHaveMultipleQueueInstancesWithLockingAsync() {
            return base.CanHaveMultipleQueueInstancesWithLockingAsync();
        }

        [Test]
        public override Task MaintainJobNotAbandon_NotWorkTimeOutEntry() {
            return base.MaintainJobNotAbandon_NotWorkTimeOutEntry();
        }
    }
}