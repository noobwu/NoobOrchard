using Orchard.Caching;
using Orchard.Logging;
using System;

namespace Orchard.Metrics
{
    public class InMemoryMetricsClient : CacheBucketMetricsClientBase
    {
        public InMemoryMetricsClient(InMemoryMetricsClientOptions options) : base(new InMemoryCacheClient(new InMemoryCacheClientOptions { LoggerFactory = options.LoggerFactory }), options)
        {

        }

        public override void Dispose()
        {
            base.Dispose();
            _cache.Dispose();
        }
    }
}
