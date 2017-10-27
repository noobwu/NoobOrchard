using Orchard.Caching;
using Orchard.Redis.Caching;
using Orchard.Tests.Caching;
using System.Threading.Tasks;
using NUnit.Framework;
namespace Orchard.Redis.Tests.Caching
{
    public class RedisCacheClientTests : CacheClientTestsBase {
        public RedisCacheClientTests(){
            var muxer = SharedConnection.GetMuxer();
            muxer.FlushAllAsync().GetAwaiter().GetResult();
        }

        protected override ICacheClient GetCacheClient() {
            return new RedisCacheClient(new RedisCacheClientOptions { ConnectionMultiplexer = SharedConnection.GetMuxer(), LoggerFactory = LogFactory });
        }

        [Test]
        public override Task CanGetAllAsync() {
            return base.CanGetAllAsync();
        }

        [Test]
        public override Task CanGetAllWithOverlapAsync() {
            return base.CanGetAllWithOverlapAsync();
        }

        [Test]
        public override Task CanSetAsync() {
            return base.CanSetAsync();
        }

        [Test]
        public override Task CanSetAndGetValueAsync() {
            return base.CanSetAndGetValueAsync();
        }

        [Test]
        public override Task CanAddAsync() {
            return base.CanAddAsync();
        }

        [Test]
        public override Task CanAddConcurrentlyAsync() {
            return base.CanAddConcurrentlyAsync();
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
        public override Task CanSetExpirationAsync() {
            return base.CanSetExpirationAsync();
        }

        [Test]
        public override Task CanIncrementAsync() {
            return base.CanIncrementAsync();
        }

        [Test]
        public override Task CanIncrementAndExpireAsync() {
            return base.CanIncrementAndExpireAsync();
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
        public override Task CanManageSetsAsync() {
            return base.CanManageSetsAsync();
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
