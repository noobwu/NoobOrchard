using System;
using StackExchange.Redis;
using Orchard.Messaging;

namespace Orchard.Redis.Messaging
{
    public class RedisMessageBusOptions : MessageBusOptionsBase {
        public ISubscriber Subscriber { get; set; }
    }
}