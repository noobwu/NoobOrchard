

using Autofac;
using NUnit.Framework;

namespace Orchard.Messaging.Tests
{
    [TestFixture]
	public class InMemoryTransientMessagingHostTests
		: TransientServiceMessagingTests
	{
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        public override void Register(ContainerBuilder builder)
        {
            base.Register(builder);
        }
        InMemoryTransientMessageService messageService;

		protected override IMessageFactory CreateMessageFactory()
		{
			messageService = new InMemoryTransientMessageService();
			return new InMemoryTransientMessageFactory(messageService);
		}

		protected override TransientMessageServiceBase CreateMessagingService()
		{
            if (messageService == null)
            {
                messageService = new InMemoryTransientMessageService();
            }
			return messageService;
		}
	}
}