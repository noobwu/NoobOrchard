using Orchard.Caching;
using Orchard.Serializer;
using StackExchange.Redis;

namespace Orchard.Redis.Caching
{
    public class RedisCacheClientOptions : CacheClientOptionsBase {
        public ConnectionMultiplexer ConnectionMultiplexer { get; set; }
        public ISerializer Serializer { get; set; }
        public string PrefixKey { get; set; } = "Orchard:Cache:";
    }
}