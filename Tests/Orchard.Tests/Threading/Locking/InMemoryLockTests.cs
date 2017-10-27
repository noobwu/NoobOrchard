using NUnit.Framework;
using Orchard.Caching;
using Orchard.Messaging;
using Orchard.Threading.Locking;
using Orchard.Utility;
using System;
using System.Threading.Tasks;

namespace Orchard.Tests.Threading.Locking
{
    public class InMemoryLockTests : LockTestBase, IDisposable {
        private readonly ICacheClient _cache;
        private readonly IMessageBus _messageBus;

        public InMemoryLockTests() {
            _cache = new InMemoryCacheClient(new InMemoryCacheClientOptions { LoggerFactory = LogFactory });
            _messageBus = new InMemoryMessageBus(new InMemoryMessageBusOptions { LoggerFactory = LogFactory });
        }

        protected override ILockProvider GetThrottlingLockProvider(int maxHits, TimeSpan period) {
            return new ThrottlingLockProvider(_cache, maxHits, period, LogFactory);
        }

        protected override ILockProvider GetLockProvider() {
            return new CacheLockProvider(_cache, _messageBus, LogFactory);
        }

        [Test]
        public override Task CanAcquireAndReleaseLockAsync() {
            using (TestSystemClock.Install()) {
                return base.CanAcquireAndReleaseLockAsync();
            }
        }

        [Test]
        public override Task LockWillTimeoutAsync() {
            return base.LockWillTimeoutAsync();
        }

        [Test]
        public override Task LockOneAtATimeAsync() {
            return base.LockOneAtATimeAsync();
        }

        [Test]
        public override Task WillThrottleCallsAsync() {
            return base.WillThrottleCallsAsync();
        }
        
        public void Dispose() {
            _cache.Dispose();
            _messageBus.Dispose();
        }
    }
}