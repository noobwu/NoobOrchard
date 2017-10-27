using Nito.AsyncEx;
using NUnit.Framework;
using Orchard.Caching;
using Orchard.Messaging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Orchard.Tests.Caching
{
    public class HybridCacheClientTests : CacheClientTestsBase, IDisposable
    {
        private readonly ICacheClient _distributedCache = new InMemoryCacheClient(new InMemoryCacheClientOptions());
        private readonly IMessageBus _messageBus = new InMemoryMessageBus(new InMemoryMessageBusOptions());



        protected override ICacheClient GetCacheClient()
        {
            return new HybridCacheClient(_distributedCache, _messageBus, LogFactory);
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
        public virtual async Task WillUseLocalCache()
        {
            using (var firstCache = GetCacheClient() as HybridCacheClient)
            {
                Assert.NotNull(firstCache);

                using (var secondCache = GetCacheClient() as HybridCacheClient)
                {
                    Assert.NotNull(secondCache);

                    await firstCache.SetAsync("first1", 1);
                    await firstCache.IncrementAsync("first2");
                    Assert.AreEqual(1, firstCache.LocalCache.Count);

                    string cacheKey = Guid.NewGuid().ToString("N").Substring(10);
                    await firstCache.SetAsync(cacheKey, new SimpleModel { Data1 = "test" });
                    Assert.AreEqual(2, firstCache.LocalCache.Count);
                    Assert.AreEqual(0, secondCache.LocalCache.Count);
                    Assert.AreEqual(0, firstCache.LocalCacheHits);

                    Assert.True((await firstCache.GetAsync<SimpleModel>(cacheKey)).HasValue);
                    Assert.AreEqual(1, firstCache.LocalCacheHits);
                    Assert.AreEqual(2, firstCache.LocalCache.Count);

                    Assert.True((await secondCache.GetAsync<SimpleModel>(cacheKey)).HasValue);
                    Assert.AreEqual(0, secondCache.LocalCacheHits);
                    Assert.AreEqual(1, secondCache.LocalCache.Count);

                    Assert.True((await secondCache.GetAsync<SimpleModel>(cacheKey)).HasValue);
                    Assert.AreEqual(1, secondCache.LocalCacheHits);
                }
            }
        }

        [Test]
        public virtual async Task WillExpireRemoteItems()
        {
            using (var firstCache = GetCacheClient() as HybridCacheClient)
            {
                Assert.NotNull(firstCache);
                var firstResetEvent = new AsyncAutoResetEvent(false);
                Action<object, ItemExpiredEventArgs> expiredHandler = (sender, args) =>
                {
                    _logger.DebugFormat("First local cache expired: {0}", args.Key);
                    firstResetEvent.Set();
                };

                using (firstCache.LocalCache.ItemExpired.AddSyncHandler(expiredHandler))
                {
                    using (var secondCache = GetCacheClient() as HybridCacheClient)
                    {
                        Assert.NotNull(secondCache);
                        var secondResetEvent = new AsyncAutoResetEvent(false);
                        Action<object, ItemExpiredEventArgs> expiredHandler2 = (sender, args) =>
                        {
                            _logger.DebugFormat("Second local cache expired: {0}", args.Key);
                            secondResetEvent.Set();
                        };

                        using (secondCache.LocalCache.ItemExpired.AddSyncHandler(expiredHandler2))
                        {
                            string cacheKey = "will-expire-remote";
                            _logger.Debug("First Set");
                            Assert.True(await firstCache.AddAsync(cacheKey, new SimpleModel { Data1 = "test" }, TimeSpan.FromMilliseconds(250)));
                            _logger.Debug("Done First Set");
                            Assert.AreEqual(1, firstCache.LocalCache.Count);

                            _logger.Debug("Second Get");
                            Assert.True((await secondCache.GetAsync<SimpleModel>(cacheKey)).HasValue);
                            _logger.Debug("Done Second Get");
                            Assert.AreEqual(1, secondCache.LocalCache.Count);

                            _logger.Debug("Waiting for item expired handlers...");
                            var sw = Stopwatch.StartNew();
                            await firstResetEvent.WaitAsync();
                            await secondResetEvent.WaitAsync();
                            sw.Stop();
                            _logger.DebugFormat("Time {0}", sw.Elapsed);
                        }
                    }
                }
            }
        }

        [Test]
        public virtual async Task WillWorkWithSets()
        {
            using (var firstCache = GetCacheClient() as HybridCacheClient)
            {
                Assert.NotNull(firstCache);

                using (var secondCache = GetCacheClient() as HybridCacheClient)
                {
                    Assert.NotNull(secondCache);

                    await firstCache.SetAddAsync("set1", new[] { 1, 2, 3 });

                    var values = await secondCache.GetSetAsync<int>("set1");

                    Assert.AreEqual(3, values.Value.Count);
                }
            }
        }

        public void Dispose()
        {
            _distributedCache.Dispose();
            _messageBus.Dispose();
        }
    }
}
