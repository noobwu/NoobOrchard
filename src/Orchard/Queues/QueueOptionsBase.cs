using Orchard.Logging;
using Orchard.Serializer;
using System;
using System.Collections.Generic;

namespace Orchard.Queues
{
    public abstract class QueueOptionsBase<T>  where T : class {
        public string Name { get; set; } = typeof(T).Name;
        public int Retries { get; set; } = 2;
        public TimeSpan WorkItemTimeout { get; set; } = TimeSpan.FromMinutes(5);
        public IEnumerable<IQueueBehavior<T>> Behaviors { get; set; }
        public ISerializer Serializer { get; set; }
        public ILoggerFactory LoggerFactory { get; set; }
    }
}