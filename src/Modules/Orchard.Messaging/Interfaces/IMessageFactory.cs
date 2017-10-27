using System;

namespace Orchard.Messaging
{
    public interface IMessageFactory : IMessageQueueClientFactory
    {
        IMessageProducer CreateMessageProducer();
    }
}