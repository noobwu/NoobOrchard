using System;

namespace Orchard.Messaging
{
    public interface IMessageQueueClientFactory
        : IDisposable
    {
        IMessageQueueClient CreateMessageQueueClient();
    }
}