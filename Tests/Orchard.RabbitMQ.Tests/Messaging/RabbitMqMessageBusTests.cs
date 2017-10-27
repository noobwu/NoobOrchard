using Orchard.Messaging;
using Orchard.RabbitMQ.Messaging;
using Orchard.Tests.Messaging;
using System.Threading.Tasks;
using NUnit.Framework;
namespace Orchard.RabbitMQ.Tests.Messaging
{
    public class RabbitMqMessageBusTests : MessageBusTestBase {
        public RabbitMqMessageBusTests(){
        }

        protected override IMessageBus GetMessageBus() {
            return new RabbitMQMessageBus(new RabbitMQMessageBusOptions { ConnectionString = "amqp://localhost", Topic = "test-messages", ExchangeName = "OrchardExchange", VirtualHost="test", LoggerFactory = LogFactory });
        }

        [Test]
        public override Task CanSendMessageAsync() {
            return base.CanSendMessageAsync();
        }

        [Test]
        public override Task CanHandleNullMessageAsync() {
            return base.CanHandleNullMessageAsync();
        }

        [Test]
        public override Task CanSendDerivedMessageAsync() {
            return base.CanSendDerivedMessageAsync();
        }

        [Test]
        public override Task CanSendDelayedMessageAsync() {
            return base.CanSendDelayedMessageAsync();
        }

        [Test]
        public override Task CanSubscribeConcurrentlyAsync() {
            return base.CanSubscribeConcurrentlyAsync();
        }

        [Ignore("TODO: Ensure this is not broken")]
        public override Task CanReceiveMessagesConcurrentlyAsync() {
            return base.CanReceiveMessagesConcurrentlyAsync();
        }

        [Test]
        public override Task CanSendMessageToMultipleSubscribersAsync() {
            return base.CanSendMessageToMultipleSubscribersAsync();
        }

        [Test]
        public override Task CanTolerateSubscriberFailureAsync() {
            return base.CanTolerateSubscriberFailureAsync();
        }

        [Test]
        public override Task WillOnlyReceiveSubscribedMessageTypeAsync() {
            return base.WillOnlyReceiveSubscribedMessageTypeAsync();
        }

        [Test]
        public override Task WillReceiveDerivedMessageTypesAsync() {
            return base.WillReceiveDerivedMessageTypesAsync();
        }

        [Test]
        public override Task CanSubscribeToAllMessageTypesAsync() {
            return base.CanSubscribeToAllMessageTypesAsync();
        }

        [Test]
        public override Task CanCancelSubscriptionAsync() {
            return base.CanCancelSubscriptionAsync();
        }

        [Test]
        public override Task WontKeepMessagesWithNoSubscribersAsync() {
            return base.WontKeepMessagesWithNoSubscribersAsync();
        }

        [Ignore("TODO: Ensure this is not broken")]
        public override Task CanReceiveFromMultipleSubscribersAsync() {
            return base.CanReceiveFromMultipleSubscribersAsync();
        }

        [Test]
        public override void CanDisposeWithNoSubscribersOrPublishers() {
            base.CanDisposeWithNoSubscribersOrPublishers();
        }
    }
}
