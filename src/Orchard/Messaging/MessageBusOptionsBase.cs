using Orchard.Logging;
using Orchard.Serializer;

namespace Orchard.Messaging
{
    public abstract class MessageBusOptionsBase {
        /// <summary>
        /// The topic name
        /// </summary>
        public string Topic { get; set; } = "messages";
        public ISerializer Serializer { get; set; }
        public ILoggerFactory LoggerFactory { get; set; }
    }
}