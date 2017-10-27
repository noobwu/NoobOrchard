using NUnit.Framework;
using Orchard.Caching;
using Orchard.Redis.Caching;
using Orchard.Tests.Caching;
using System.Threading.Tasks;

namespace Orchard.Redis.Tests.Caching
{
    public class RedisHybridCacheClientTests : HybridCacheClientTests {
        public RedisHybridCacheClientTests(){
            var muxer = SharedConnection.GetMuxer();
            muxer.FlushAllAsync().GetAwaiter().GetResult();
        }

        protected override ICacheClient GetCacheClient() {
            return new RedisHybridCacheClient(SharedConnection.GetMuxer(), loggerFactory: LogFactory);
        }

        [Test]
        public override Task CanSetAndGetValueAsync() {
            return base.CanSetAndGetValueAsync();
        }

        [Test]
        public override Task CanSetAndGetObjectAsync() {
            return base.CanSetAndGetObjectAsync();
        }
        
        [Test]
        public override Task CanTryGetAsync() {
            return base.CanTryGetAsync();
        }

        [Test]
        public override Task CanRemoveByPrefixAsync() {
            return base.CanRemoveByPrefixAsync();
        }

        [Test]
        public override Task CanUseScopedCachesAsync() {
            return base.CanUseScopedCachesAsync();
        }

        [Test]
        public override Task CanSetExpirationAsync() {
            return base.CanSetExpirationAsync();
        }
        
        [Test]
        public override Task CanManageSetsAsync() {
            return base.CanManageSetsAsync();
        }

        [Test]
        public override Task WillUseLocalCache() {
            return base.WillUseLocalCache();
        }

        [Test]
        public override Task WillExpireRemoteItems() {
            return base.WillExpireRemoteItems();
        }

        [Test]
        public override Task WillWorkWithSets() {
            return base.WillWorkWithSets();
        }

        [Ignore("Performance Test")]
        public override Task MeasureThroughputAsync() {
            return base.MeasureThroughputAsync();
        }

        [Ignore("Performance Test")]
        public override Task MeasureSerializerSimpleThroughputAsync() {
            return base.MeasureSerializerSimpleThroughputAsync();
        }

        [Ignore("Performance Test")]
        public override Task MeasureSerializerComplexThroughputAsync() {
            return base.MeasureSerializerComplexThroughputAsync();
        }
    }
}
