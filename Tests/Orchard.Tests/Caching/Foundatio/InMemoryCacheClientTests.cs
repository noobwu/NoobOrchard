using NUnit.Framework;
using Orchard.Caching;
using Orchard.Utility;
using System;
using System.Threading.Tasks;

namespace Orchard.Tests.Caching
{
    public class InMemoryCacheClientTests : CacheClientTestsBase
    {

        protected override ICacheClient GetCacheClient()
        {
            return new InMemoryCacheClient(new InMemoryCacheClientOptions { LoggerFactory = LogFactory });
        }

        [Test]
        public override Task CanGetAllAsync()
        {
            return base.CanGetAllAsync();
        }

        [Test]
        public override Task CanGetAllWithOverlapAsync()
        {
            return base.CanGetAllWithOverlapAsync();
        }

        [Test]
        public override Task CanSetAsync()
        {
            return base.CanSetAsync();
        }

        [Test]
        public override Task CanSetAndGetValueAsync()
        {
            return base.CanSetAndGetValueAsync();
        }

        [Test]
        public override Task CanAddAsync()
        {
            return base.CanAddAsync();
        }

        [Test]
        public override Task CanAddConcurrentlyAsync()
        {
            return base.CanAddConcurrentlyAsync();
        }

        [Test]
        public override Task CanTryGetAsync()
        {
            return base.CanTryGetAsync();
        }

        [Test]
        public override Task CanUseScopedCachesAsync()
        {
            return base.CanUseScopedCachesAsync();
        }

        [Test]
        public override Task CanSetAndGetObjectAsync()
        {
            return base.CanSetAndGetObjectAsync();
        }

        [Test]
        public override Task CanRemoveByPrefixAsync()
        {
            return base.CanRemoveByPrefixAsync();
        }

        [Test]
        public override Task CanSetExpirationAsync()
        {
            return base.CanSetExpirationAsync();
        }

        [Test]
        public override Task CanIncrementAsync()
        {
            return base.CanIncrementAsync();
        }

        [Test]
        public override Task CanIncrementAndExpireAsync()
        {
            return base.CanIncrementAndExpireAsync();
        }

        [Test]
        public override Task CanManageSetsAsync()
        {
            return base.CanManageSetsAsync();
        }

        [Test]
        public async Task CanSetMaxItems()
        {
            // run in tight loop so that the code is warmed up and we can catch timing issues
            for (int x = 0; x < 5; x++)
            {
                var cache = GetCacheClient() as InMemoryCacheClient;
                if (cache == null)
                    return;

                using (cache)
                {
                    await cache.RemoveAllAsync();

                    cache.MaxItems = 10;
                    for (int i = 0; i < cache.MaxItems; i++)
                        await cache.SetAsync("test" + i, i);

                    _logger.Debug(String.Join(",", cache.Keys));
                    Assert.AreEqual(10, cache.Count);
                    await cache.SetAsync("next", 1);
                    _logger.Debug(String.Join(",", cache.Keys));
                    Assert.AreEqual(10, cache.Count);
                    Assert.False((await cache.GetAsync<int>("test0")).HasValue);
                    Assert.AreEqual(1, cache.Misses);
                    await SystemClock.SleepAsync(50); // keep the last access ticks from being the same for all items
                    Assert.NotNull(await cache.GetAsync<int?>("test1"));
                    Assert.AreEqual(1, cache.Hits);
                    await cache.SetAsync("next2", 2);
                    _logger.Debug(String.Join(",", cache.Keys));
                    Assert.False((await cache.GetAsync<int>("test2")).HasValue);
                    Assert.AreEqual(2, cache.Misses);
                    Assert.True((await cache.GetAsync<int>("test1")).HasValue);
                    Assert.AreEqual(2, cache.Misses);
                }
            }
        }
    }
}