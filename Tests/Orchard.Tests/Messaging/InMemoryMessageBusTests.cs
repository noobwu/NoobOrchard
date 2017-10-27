using NUnit.Framework;
using Orchard.Messaging;
using System;
using System.Threading.Tasks;

namespace Orchard.Tests.Messaging
{
    public class InMemoryMessageBusTests : MessageBusTestBase, IDisposable {
        private IMessageBus _messageBus;

        protected override IMessageBus GetMessageBus() {
            if (_messageBus != null)
                return _messageBus;

            _messageBus = new InMemoryMessageBus(new InMemoryMessageBusOptions { LoggerFactory = LogFactory });
            return _messageBus;
        }

        protected override Task CleanupMessageBusAsync(IMessageBus messageBus) {
            return Task.CompletedTask;
        }

       [Test]
        public async Task CanCheckMessageCounts() {
            var messageBus = new InMemoryMessageBus(new InMemoryMessageBusOptions { LoggerFactory = LogFactory });
            await messageBus.PublishAsync(new SimpleMessageA {
                Data = "Hello"
            });
           Assert.AreEqual(1, messageBus.MessagesSent);
           Assert.AreEqual(1, messageBus.GetMessagesSent<SimpleMessageA>());
           Assert.AreEqual(0, messageBus.GetMessagesSent<SimpleMessageB>());
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

       [Test]
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

       [Test]
        public override Task CanReceiveFromMultipleSubscribersAsync() {
            return base.CanReceiveFromMultipleSubscribersAsync();
        }

       [Test]
        public override void CanDisposeWithNoSubscribersOrPublishers() {
            base.CanDisposeWithNoSubscribersOrPublishers();
        }

        public void Dispose() {
            _messageBus?.Dispose();
            _messageBus = null;
        }
    }
}