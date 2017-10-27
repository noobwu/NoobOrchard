using Orchard.Caching;
using Orchard.Messaging;
using Orchard.Redis.Caching;
using Orchard.Redis.Messaging;
using Orchard.Tests.Threading.Locking;
using Orchard.Threading.Locking;
using System;
using System.Threading.Tasks;
using NUnit.Framework;
namespace Orchard.Redis.Tests.Locking
{
    public class RedisLockTests : LockTestBase, IDisposable {
        private readonly ICacheClient _cache;
        private readonly IMessageBus _messageBus;

        public RedisLockTests(){
            var muxer = SharedConnection.GetMuxer();
            muxer.FlushAllAsync().GetAwaiter().GetResult();
            _cache = new RedisCacheClient(new RedisCacheClientOptions { ConnectionMultiplexer = muxer, LoggerFactory = LogFactory });
            _messageBus = new RedisMessageBus(new RedisMessageBusOptions { Subscriber = muxer.GetSubscriber(), Topic = "test-lock", LoggerFactory = LogFactory });
        }

        protected override ILockProvider GetThrottlingLockProvider(int maxHits, TimeSpan period) {
            return new ThrottlingLockProvider(_cache, maxHits, period, LogFactory);
        }

        protected override ILockProvider GetLockProvider() {
            return new CacheLockProvider(_cache, _messageBus, LogFactory);
        }

       [Test]
        public override Task CanAcquireAndReleaseLockAsync() {
            return base.CanAcquireAndReleaseLockAsync();
        }

       [Test]
        public override Task LockWillTimeoutAsync() {
            return base.LockWillTimeoutAsync();
        }

       [Test]
        public override Task WillThrottleCallsAsync() {
            return base.WillThrottleCallsAsync();
        }

       [Test]
        public override Task LockOneAtATimeAsync() {
            return base.LockOneAtATimeAsync();
        }

        public void Dispose() {
            _cache.Dispose();
            _messageBus.Dispose();
            var muxer = SharedConnection.GetMuxer();
            muxer.FlushAllAsync().GetAwaiter().GetResult();
        }
    }
}
