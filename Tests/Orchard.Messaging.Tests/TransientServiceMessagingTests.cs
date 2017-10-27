using Autofac;
using NUnit.Framework;
using Orchard.Messaging.Tests.Services;
using System;
using System.Threading;

namespace Orchard.Messaging.Tests
{
    public abstract class TransientServiceMessagingTests
        : MessagingHostTestBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        public override void Register(ContainerBuilder builder)
        {
            builder.Register(c => new GreetService());
            builder.Register(c => new AlwaysFailService());
            builder.Register(c => new UnRetryableFailService());
        }
        public override void OnBeforeEachTest()
        {
            base.OnBeforeEachTest();
        }

        [Test]
        public void Normal_GreetService_client_and_server_example()
        {
            var service = Container.Resolve<GreetService>();
            using (var serviceHost = CreateMessagingService())
            {
                serviceHost.RegisterHandler<Greet>(m => service.Any(m.GetBody()));

                serviceHost.Start();

                using (var client = serviceHost.MessageFactory.CreateMessageQueueClient())
                {
                    client.Publish(new Greet { Name = "World!" });
                }

                Assert.That(service.Result, Is.EqualTo("Hello, World!"));
                Assert.That(service.TimesCalled, Is.EqualTo(1));
            }
        }

        [Test]
        public void Publish_before_starting_host_GreetService_client_and_server_example()
        {
            var service = Container.Resolve<GreetService>();
            using (var serviceHost = CreateMessagingService())
            {
                var request = new Greet { Name = "World!" };
                using (var client = serviceHost.MessageFactory.CreateMessageQueueClient())
                {
                    client.Publish(request);
                }

                serviceHost.RegisterHandler<Greet>(m => service.Any(m.GetBody()));
                serviceHost.Start();

                Assert.That(service.Result, Is.EqualTo("Hello, World!"));
                Assert.That(service.TimesCalled, Is.EqualTo(1));
            }
        }

        [Test]
        public void AlwaysFailsService_ends_up_in_dlq_after_3_attempts()
        {
            var service = Container.Resolve<AlwaysFailService>();
            var request = new AlwaysFail { Name = "World!" };
            using (var serviceHost = CreateMessagingService())
            {
                using (var client = serviceHost.MessageFactory.CreateMessageQueueClient())
                {
                    client.Publish(request);
                }
                serviceHost.RegisterHandler<AlwaysFail>(m => service.Any(m.GetBody()));
                serviceHost.Start();
                Assert.That(service.Result, Is.Null);
                Assert.That(service.TimesCalled, Is.EqualTo(3));


                using (var client = serviceHost.MessageFactory.CreateMessageQueueClient())
                {
                    var dlqMessage = client.GetAsync<AlwaysFail>(QueueNames<AlwaysFail>.Dlq);
                    client.Ack(dlqMessage);

                    Assert.That(dlqMessage, Is.Not.Null);
                    Assert.That(dlqMessage.GetBody().Name, Is.EqualTo(request.Name));
                }
            }

        }

        [Test]
        public void UnRetryableFailService_ends_up_in_dlq_after_1_attempt()
        {
            var service = Container.Resolve<UnRetryableFailService>();
            var request = new UnRetryableFail { Name = "World!" };
            using (var serviceHost = CreateMessagingService())
            {
                using (var client = serviceHost.MessageFactory.CreateMessageQueueClient())
                {
                    client.Publish(request);
                }

                serviceHost.RegisterHandler<UnRetryableFail>(m => service.Any(m.GetBody()));
                serviceHost.Start();

                Assert.That(service.Result, Is.Null);
                Assert.That(service.TimesCalled, Is.EqualTo(1));

                using (var client = serviceHost.MessageFactory.CreateMessageQueueClient())
                {
                    var dlqMessage = client.GetAsync<UnRetryableFail>(QueueNames<UnRetryableFail>.Dlq);
                    client.Ack(dlqMessage);

                    Assert.That(dlqMessage, Is.Not.Null);
                    Assert.That(dlqMessage.GetBody().Name, Is.EqualTo(request.Name));
                }
            }
        }

    }
}