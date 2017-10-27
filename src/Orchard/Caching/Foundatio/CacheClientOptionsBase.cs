using Orchard.Logging;

namespace Orchard.Caching
{
    public abstract class CacheClientOptionsBase {
        public ILoggerFactory LoggerFactory { get; set; }
    }
}