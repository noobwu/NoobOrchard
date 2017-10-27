using System;

namespace Orchard.Messaging {
    public interface IMessageBus : IMessagePublisher, IMessageSubscriber, IDisposable {}
}