
using Autofac;
using NUnit.Framework;
using Orchard.Logging;

namespace Orchard.Messaging.Tests
{
	[TestFixture]
	public abstract class MessagingHostTestBase
	{
		protected abstract IMessageFactory CreateMessageFactory();

		protected abstract TransientMessageServiceBase CreateMessagingService();
        /// <summary>
        /// 
        /// </summary>
		protected IContainer Container { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        public abstract void Register(ContainerBuilder builder);
        [SetUp]
		public virtual void OnBeforeEachTest()
		{
            LoggerManager.LogFactory = new ConsoleLoggerFactory();
			if (Container != null)
			{
				Container.Dispose();
			}
            ContainerBuilder builder = new ContainerBuilder();
            builder.Register<IMessageFactory>(a => CreateMessageFactory());
            Register(builder);
            Container = builder.Build();
		}

	}
}
