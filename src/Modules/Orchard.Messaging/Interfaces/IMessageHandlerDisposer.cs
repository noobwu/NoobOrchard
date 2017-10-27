namespace Orchard.Messaging
{
    public interface IMessageHandlerDisposer
    {
        void DisposeMessageHandler(IMessageHandler messageHandler);
    }
}