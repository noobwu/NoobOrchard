using System;

namespace Orchard.Messaging
{
    public interface IMessageProducer
        : IDisposable
    {
        void Publish<T>(T messageBody);
        void Publish<T>(IMessage<T> message);
    }

}
