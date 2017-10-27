using System;
using StackExchange.Redis;
using Orchard.Metrics;

namespace Orchard.Redis.Metrics {
    public class RedisMetricsClientOptions : MetricsClientOptionsBase {
        public ConnectionMultiplexer ConnectionMultiplexer { get; set; }
    }
}