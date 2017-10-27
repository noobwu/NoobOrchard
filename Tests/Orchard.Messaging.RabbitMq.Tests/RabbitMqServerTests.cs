using NUnit.Framework;
using Orchard.Client;
using Orchard.Logging;
using RabbitMQ.Client;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Orchard.Messaging.RabbitMq.Tests
{
    public class Reverse
    {
        public string Value { get; set; }
    }

    public class Rot13
    {
        public string Value { get; set; }
    }

    public class AlwaysThrows
    {
        public string Value { get; set; }
    }

    [TestFixture, Category("RabbitMQ"), Category("Messaging")]
    public class RabbitMqServerTests
    {
        static readonly string ConnectionString = Config.RabbitMQConnString;

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            LoggerManager.LogFactory = new ConsoleLoggerFactory();
        }

        internal static RabbitMqServer CreateMqServer(int noOfRetries = 2)
        {
            var mqHost = new RabbitMqServer(ConnectionString, virtualHost: Config.RabbitMQVirtualHost);
            return mqHost;
        }

        internal static void Publish_4_messages(IMessageQueueClient mqClient)
        {
            mqClient.Publish(new Reverse { Value = "Hello" });
            mqClient.Publish(new Reverse { Value = "World" });
            mqClient.Publish(new Reverse { Value = "Orchard" });
            mqClient.Publish(new Reverse { Value = "Redis" });
        }

        private static void Publish_4_Rot13_messages(IMessageQueueClient mqClient)
        {
            mqClient.Publish(new Rot13 { Value = "Hello" });
            mqClient.Publish(new Rot13 { Value = "World" });
            mqClient.Publish(new Rot13 { Value = "Orchard" });
            mqClient.Publish(new Rot13 { Value = "Redis" });
        }

        [Test]
        public void Utils_publish_Reverse_messages()
        {
            using (var mqHost = new RabbitMqServer(ConnectionString,virtualHost:Config.RabbitMQVirtualHost))
            using (var mqClient = mqHost.CreateMessageQueueClient())
            {
                Publish_4_messages(mqClient);
            }
        }

        [Test]
        public void Utils_publish_Rot13_messages()
        {
            using (var mqHost = new RabbitMqServer(ConnectionString,virtualHost:Config.RabbitMQVirtualHost))
            using (var mqClient = mqHost.CreateMessageQueueClient())
            {
                Publish_4_Rot13_messages(mqClient);
            }
        }

        [Test]
        public void Only_allows_1_BgThread_to_run_at_a_time()
        {
            using (var mqHost = CreateMqServer())
            {
                mqHost.RegisterHandler<Reverse>(x => x.GetBody().Value.Reverse());
                mqHost.RegisterHandler<Rot13>(x => x.GetBody().Value.ToRot13());

                5.Times(x => ThreadPool.QueueUserWorkItem(y => mqHost.Start()));
                ExecUtils.RetryOnException(() =>
                {
                    Assert.That(mqHost.GetStatus(), Is.EqualTo("Started"));
                    Assert.That(mqHost.BgThreadCount, Is.EqualTo(1));
                    Thread.Sleep(100);
                }, TimeSpan.FromSeconds(5));

                10.Times(x => ThreadPool.QueueUserWorkItem(y => mqHost.Stop()));
                ExecUtils.RetryOnException(() =>
                {
                    Assert.That(mqHost.GetStatus(), Is.EqualTo("Stopped"));
                    Thread.Sleep(100);
                }, TimeSpan.FromSeconds(5));

                ThreadPool.QueueUserWorkItem(y => mqHost.Start());
                ExecUtils.RetryOnException(() =>
                {
                    Assert.That(mqHost.GetStatus(), Is.EqualTo("Started"));
                    Assert.That(mqHost.BgThreadCount, Is.EqualTo(2));
                    Thread.Sleep(100);
                }, TimeSpan.FromSeconds(5));

                //Debug.WriteLine(mqHost.GetStats());
            }
        }

        [Test]
        public void Cannot_Start_a_Disposed_MqHost()
        {
            var mqHost = CreateMqServer();

            mqHost.RegisterHandler<Reverse>(x => x.GetBody().Value.Reverse());
            mqHost.Dispose();

            try
            {
                mqHost.Start();
                Assert.Fail("Should throw ObjectDisposedException");
            }
            catch (ObjectDisposedException ex)
            {
                Console.WriteLine(ex);
            }
        }

        [Test]
        public void Cannot_Stop_a_Disposed_MqHost()
        {
            var mqHost = CreateMqServer();

            mqHost.RegisterHandler<Reverse>(x => x.GetBody().Value.Reverse());
            mqHost.Start();
            Thread.Sleep(100);

            mqHost.Dispose();

            try
            {
                mqHost.Stop();
                Assert.Fail("Should throw ObjectDisposedException");
            }
            catch (ObjectDisposedException ex)
            {
                Console.WriteLine(ex);

            }
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex);
            //}
        }

        public class Incr
        {
            public int Value { get; set; }
        }

        [Test]
        public void Can_receive_and_process_same_reply_responses()
        {
            var called = 0;
            using (var mqHost = CreateMqServer())
            {

                using (var conn = mqHost.ConnectionFactory.CreateConnection())
                using (var channel = conn.CreateModel())
                {
                    channel.PurgeQueue<Incr>();
                }

                mqHost.RegisterHandler<Incr>(m =>
                {
                    var result = m.GetBody().Value;
                    called = result;
                    Console.WriteLine("In Incr #" + result);
                    Interlocked.Increment(ref called);
                    return result > 0 ? new Incr { Value = m.GetBody().Value - 1 } : null;
                });

                mqHost.Start();

                var incr = new Incr { Value = 5 };
                using (var mqClient = mqHost.CreateMessageQueueClient())
                {
                    mqClient.Publish(incr);
                }
                Thread.Sleep(100);
                ExecUtils.RetryOnException(() =>
                {
                    Assert.That(called, Is.EqualTo(1 + incr.Value));
                    Thread.Sleep(100);
                }, TimeSpan.FromSeconds(5));
            }
        }

        public class Hello : IReturn<HelloResponse>
        {
            public string Name { get; set; }
        }
        public class HelloNull : IReturn<HelloResponse>
        {
            public string Name { get; set; }
        }
        public class HelloResponse
        {
            public string Result { get; set; }
        }

        [Test]
        public void Can_receive_and_process_standard_request_reply_combo()
        {
            using (var mqHost = CreateMqServer())
            {
                using (var conn = mqHost.ConnectionFactory.CreateConnection())
                using (var channel = conn.CreateModel())
                {
                    channel.PurgeQueue<Hello>();
                    channel.PurgeQueue<HelloResponse>();
                }

                string messageReceived = null;
                string messageReceived1 = null;

                mqHost.RegisterHandler<Hello>(m =>
                   {
                       messageReceived = "Hello, " + m.GetBody().Name;
                       HelloResponse helloResult = new HelloResponse { Result = messageReceived };
                       return helloResult;
                   });

                mqHost.RegisterHandler<HelloResponse>(m =>
                {
                    messageReceived1 = m.GetBody().Result;
                    return null;
                });

                mqHost.Start();
                using (var mqClient = mqHost.CreateMessageQueueClient())
                {
                    var dto = new Hello { Name = "Orchard" };
                    mqClient.Publish(dto);
                    ExecUtils.RetryOnException(() =>
                    {
                        Assert.That(messageReceived, Is.EqualTo("Hello, Orchard"));
                        Thread.Sleep(100);
                    }, TimeSpan.FromSeconds(5));
                }
            }
        }

        public class Wait
        {
            public int ForMs { get; set; }
        }

        [Test]
        public void Can_handle_requests_concurrently_in_4_threads()
        {
            RunHandlerOnMultipleThreads(noOfThreads: 4, msgs: 10);
        }

        private static void RunHandlerOnMultipleThreads(int noOfThreads, int msgs)
        {
            using (var mqHost = CreateMqServer())
            {
                var timesCalled = 0;
                using (var conn = mqHost.ConnectionFactory.CreateConnection())
                using (var channel = conn.CreateModel())
                {
                    channel.PurgeQueue<Wait>();
                }

                mqHost.RegisterHandler<Wait>(m =>
                {
                    Interlocked.Increment(ref timesCalled);
                    Thread.Sleep(m.GetBody().ForMs);
                    return null;
                }, noOfThreads);

                mqHost.Start();

                using (var mqClient = mqHost.CreateMessageQueueClient())
                {
                    var dto = new Wait { ForMs = 100 };
                    msgs.Times(i => mqClient.Publish(dto));

                    ExecUtils.RetryOnException(() =>
                    {
                        Assert.That(timesCalled, Is.EqualTo(msgs));
                        Thread.Sleep(100);
                    }, TimeSpan.FromSeconds(5));
                }
            }
        }

        [Test]
        public void Can_publish_and_receive_messages_with_MessageFactory()
        {
            using (var mqFactory = new RabbitMqMessageFactory(Config.RabbitMQConnString,virtualHost:Config.RabbitMQVirtualHost))
            using (var mqClient = mqFactory.CreateMessageQueueClient())
            {
                mqClient.Publish(new Hello { Name = "Foo" });
                var msg = mqClient.Get<Hello>(QueueNames<Hello>.In);

                Assert.That(msg.GetBody().Name, Is.EqualTo("Foo"));
            }
        }
        [Test]
        public void Published_and_received_messages()
        {
            string receivedMsgApp = null;
            string receivedMsgType = null;
            using (var mqServer = CreateMqServer())
            {
                mqServer.PublishMessageFilter = (queueName, properties, msg) =>
                {
                    properties.AppId = string.Format("app:{0}", queueName);
                };
                mqServer.GetMessageFilter = (queueName, basicMsg) =>
                {
                    var props = basicMsg.BasicProperties;
                    receivedMsgType = props.Type; //automatically added by RabbitMqProducer
                    receivedMsgApp = props.AppId;
                };

                mqServer.RegisterHandler<Hello>(m =>
                {
                    return new HelloResponse { Result = string.Format("Hello, {0}!", m.GetBody().Name) };
                });

                mqServer.Start();

                using (var mqClient = mqServer.CreateMessageQueueClient())
                {
                    mqClient.Publish(new Hello { Name = "Bugs Bunny" });
                }

                Thread.Sleep(100);
                Assert.That(receivedMsgApp, Is.EqualTo("app:{0}".Fmt(QueueNames<Hello>.In)));
                Assert.That(receivedMsgType, Is.EqualTo(typeof(Hello).Name));
                Console.WriteLine($"receivedMsgApp:{receivedMsgApp},receivedMsgType:{receivedMsgType}");
                var factory = mqServer.ConnectionFactory;
                using (IConnection connection = factory.CreateConnection())
                using (IModel channel = connection.CreateModel())
                {
                    var queueName = QueueNames<Hello>.In;//mq:Hello.inq
                    channel.RegisterQueue(queueName);
                    Thread.Sleep(1000);

                    var basicMsg = channel.BasicGet(queueName, autoAck: false);
                    if (basicMsg != null)
                    {
                        Console.WriteLine("app:{0}".Fmt(queueName) + "," + basicMsg);
                        var props = basicMsg.BasicProperties;

                        Assert.That(props.Type, Is.EqualTo(typeof(HelloResponse).Name));
                        Assert.That(props.AppId, Is.EqualTo("app:{0}".Fmt(queueName)));

                        var msg = basicMsg.ToMessage<HelloResponse>();

                        Assert.That(msg.GetBody().Result, Is.EqualTo("Hello, Bugs Bunny!"));
                    }
                }
            }
        }
        [Test]
        public void Can_filter_published_and_received_messages()
        {
            try
            {

                string receivedMsgApp = null;
                string receivedMsgType = null;
                Random random = new Random();
                int randId = random.Next(1000, 9999);
                using (var mqServer = CreateMqServer())
                {
                    mqServer.PublishMessageFilter = (queueName, properties, msg) =>
                    {
                        properties.AppId = string.Format("app:{0}", queueName);
                    };
                    mqServer.GetMessageFilter = (queueName, basicMsg) =>
                    {
                        var props = basicMsg.BasicProperties;
                        receivedMsgType = props.Type; //automatically added by RabbitMqProducer
                        receivedMsgApp = props.AppId;
                    };

                    mqServer.RegisterHandler<Hello>(m =>
                    {
                        return new HelloResponse { Result = string.Format("Hello, {0}!", m.GetBody().Name) };
                    });

                    mqServer.Start();

                    using (var mqClient = mqServer.CreateMessageQueueClient())
                    {
                        mqClient.Publish(new Hello { Name = "Bugs Bunny" });
                    }

                    Thread.Sleep(100);
                    Assert.That(receivedMsgApp, Is.EqualTo("app:{0}".Fmt(QueueNames<Hello>.In)));
                    Assert.That(receivedMsgType, Is.EqualTo(typeof(Hello).Name));

                    var factory = mqServer.ConnectionFactory;

                    using (var mqClient = mqServer.CreateMessageQueueClient())
                    {
                        mqClient.Publish(new HelloResponse { Result = "Bugs Bunny" });
                    }

                    Thread.Sleep(100);

                    using (IConnection connection = factory.CreateConnection())
                    using (IModel channel = connection.CreateModel())
                    {
                        var queueName = QueueNames<HelloResponse>.In;//mq:HelloResponse.inq
                        channel.RegisterQueue(queueName);
                        Thread.Sleep(1000);

                        var basicMsg = channel.BasicGet(queueName, autoAck: false);
                        if (basicMsg != null)
                        {
                            Console.WriteLine("app:{0}".Fmt(queueName) + "," + basicMsg);
                            var props = basicMsg.BasicProperties;

                            Assert.That(props.Type, Is.EqualTo(typeof(HelloResponse).Name));
                            Assert.That(props.AppId, Is.EqualTo("app:{0}".Fmt(queueName)));

                            var msg = basicMsg.ToMessage<HelloResponse>();

                            Assert.That(msg.GetBody().Result, Is.EqualTo("Bugs Bunny"));
                        }
                    }
                    string testQueueName = "hello";
                    string testMsg = "Hello Word! randId:" + randId;
                    using (var connection = factory.CreateConnection())//创建连接对象，基于 Socket
                    using (var channel = connection.CreateModel())//创建新的渠道、会话
                    {
                        //声明队列
                        channel.QueueDeclare(queue: testQueueName,//队列名
                                             durable: true,//持久性
                                             exclusive: false,//排他性
                                             autoDelete: false,//自动删除
                                             arguments: null);//参数
                        var body = Encoding.UTF8.GetBytes(testMsg);

                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;
                        channel.BasicPublish(exchange: "",//交换机名
                            routingKey: testQueueName, //路由键
                            basicProperties: properties,
                            body: body);
                        Console.WriteLine(string.Format("Work Queues {0} Sent {1}", testQueueName, testMsg));


                    }
                    Thread.Sleep(100);
                    //var factory = new ConnectionFactory() { HostName = "localhost", VirtualHost = "/test", UserName = "test", Password = "test" };
                    using (IConnection connection = factory.CreateConnection())
                    using (IModel channel = connection.CreateModel())
                    {
                        ////声明队列
                        //channel.QueueDeclare(queue: testQueueName,//队列名
                        //                     durable: true,//持久性
                        //                     exclusive: false,//排他性
                        //                     autoDelete: false,//自动删除
                        //                     arguments: null);//参数

                        ////创建基于该队列的消费者，绑定事件
                        //var consumer = new EventingBasicConsumer(channel);
                        //consumer.Received += (model, ea) =>
                        //{
                        //    var body = ea.Body;//消息主体
                        //    var message = Encoding.UTF8.GetString(body);
                        //    Console.WriteLine("Hello World [x] Received {0}", message);
                        //};
                        ////启动消费者
                        //channel.BasicConsume(queue: testQueueName,//队列名
                        //                     autoAck: true,//false：手动应答；true：自动应答
                        //                     consumer: consumer);
                        //Thread.Sleep(100); ;//不能去掉

                        var testBasicMsg = channel.BasicGet(queue: testQueueName, autoAck: true);
                        var testReceivedResult = Encoding.UTF8.GetString(testBasicMsg.Body);
                        Console.WriteLine("{0} queue Received {1}", testQueueName, testMsg);
                        Assert.That(testReceivedResult, Is.EqualTo(testMsg));
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }



        }

        [Test]
        public void Messages_with_null_Response_is_published_to_OutMQ()
        {
            int msgsReceived = 0;
            using (var mqServer = CreateMqServer())
            {
                mqServer.RegisterHandler<HelloNull>(m =>
                {
                    Interlocked.Increment(ref msgsReceived);
                    return null;
                });

                mqServer.Start();

                using (var mqClient = mqServer.CreateMessageQueueClient())
                {
                    mqClient.Publish(new HelloNull { Name = "Into the Void" });

                    var msg = mqClient.Get<HelloNull>(QueueNames<HelloNull>.Out, TimeSpan.FromSeconds(5));
                    Assert.That(msg, Is.Not.Null);

                    HelloNull response = msg.GetBody();

                    Thread.Sleep(100);

                    Assert.That(response.Name, Is.EqualTo("Into the Void"));
                    Assert.That(msgsReceived, Is.EqualTo(1));
                }
            }
        }

        [Test]
        public void Messages_with_null_Response_is_published_to_ReplyMQ()
        {
            int msgsReceived = 0;
            using (var mqServer = CreateMqServer())
            {
                mqServer.RegisterHandler<HelloNull>(m =>
                {
                    Interlocked.Increment(ref msgsReceived);
                    return null;
                });

                mqServer.Start();

                using (var mqClient = mqServer.CreateMessageQueueClient())
                {
                    var replyMq = mqClient.GetTempQueueName();
                    mqClient.Publish(new Message<HelloNull>(new HelloNull { Name = "Into the Void" })
                    {
                        ReplyTo = replyMq
                    });
                    Thread.Sleep(100);

                    var msg = mqClient.Get<HelloNull>(replyMq);

                    HelloNull response = msg.GetBody();


                    Assert.That(response.Name, Is.EqualTo("Into the Void"));
                    Assert.That(msgsReceived, Is.EqualTo(1));
                }
            }
        }
    }

    [Explicit("These Flaky tests pass when run manually")]
    [TestFixture, Category("Integration")]
    public class RabbitMqServerFragileTests
    {
        [Test]
        public void Does_process_all_messages_and_Starts_Stops_correctly_with_multiple_threads_racing()
        {
            using (var mqHost = RabbitMqServerTests.CreateMqServer())
            {
                using (var conn = mqHost.ConnectionFactory.CreateConnection())
                using (var channel = conn.CreateModel())
                {
                    channel.PurgeQueue<Reverse>();
                    channel.PurgeQueue<Rot13>();
                }

                var reverseCalled = 0;
                var rot13Called = 0;

                mqHost.RegisterHandler<Reverse>(x =>
                {
                    "Processing Reverse {0}...".Print(Interlocked.Increment(ref reverseCalled));
                    return x.GetBody().Value.Reverse();
                });
                mqHost.RegisterHandler<Rot13>(x =>
                {
                    "Processing Rot13 {0}...".Print(Interlocked.Increment(ref rot13Called));
                    return x.GetBody().Value.ToRot13();
                });

                using (var mqClient = mqHost.CreateMessageQueueClient())
                {
                    mqClient.Publish(new Reverse { Value = "Hello" });
                    mqClient.Publish(new Reverse { Value = "World" });
                    mqClient.Publish(new Rot13 { Value = "Orchard" });

                    mqHost.Start();

                    ExecUtils.RetryOnException(() =>
                    {
                        Assert.That(mqHost.GetStatus(), Is.EqualTo("Started"));
                        Assert.That(mqHost.GetStats().TotalMessagesProcessed, Is.EqualTo(3));
                        Thread.Sleep(100);
                    }, TimeSpan.FromSeconds(5));

                    mqClient.Publish(new Reverse { Value = "Foo" });
                    mqClient.Publish(new Rot13 { Value = "Bar" });

                    10.Times(x => ThreadPool.QueueUserWorkItem(y => mqHost.Start()));
                    Assert.That(mqHost.GetStatus(), Is.EqualTo("Started"));

                    5.Times(x => ThreadPool.QueueUserWorkItem(y => mqHost.Stop()));
                    ExecUtils.RetryOnException(() =>
                    {
                        Assert.That(mqHost.GetStatus(), Is.EqualTo("Stopped"));
                        Thread.Sleep(100);
                    }, TimeSpan.FromSeconds(5));

                    10.Times(x => ThreadPool.QueueUserWorkItem(y => mqHost.Start()));
                    ExecUtils.RetryOnException(() =>
                    {
                        Assert.That(mqHost.GetStatus(), Is.EqualTo("Started"));
                        Thread.Sleep(100);
                    }, TimeSpan.FromSeconds(5));

                    Debug.WriteLine("\n" + mqHost.GetStats());

                    Assert.That(mqHost.GetStats().TotalMessagesProcessed, Is.GreaterThanOrEqualTo(5));
                    Assert.That(reverseCalled, Is.EqualTo(3));
                    Assert.That(rot13Called, Is.EqualTo(2));
                }
            }
        }


        [Test]
        public void Does_retry_messages_with_errors_by_RetryCount()
        {
            var retryCount = 1;
            var totalRetries = 1 + retryCount; //in total, inc. first try

            using (var mqHost = RabbitMqServerTests.CreateMqServer(retryCount))
            {
                using (var conn = mqHost.ConnectionFactory.CreateConnection())
                using (var channel = conn.CreateModel())
                {
                    channel.PurgeQueue<Reverse>();
                    channel.PurgeQueue<Rot13>();
                    channel.PurgeQueue<AlwaysThrows>();
                }

                var reverseCalled = 0;
                var rot13Called = 0;

                mqHost.RegisterHandler<Reverse>(x => { Interlocked.Increment(ref reverseCalled); return x.GetBody().Value.Reverse(); });
                mqHost.RegisterHandler<Rot13>(x => { Interlocked.Increment(ref rot13Called); return x.GetBody().Value.ToRot13(); });
                mqHost.RegisterHandler<AlwaysThrows>(x => { throw new Exception("Always Throwing! " + x.GetBody().Value); });
                mqHost.Start();

                using (var mqClient = mqHost.CreateMessageQueueClient())
                {
                    mqClient.Publish(new AlwaysThrows { Value = "1st" });
                    mqClient.Publish(new Reverse { Value = "Hello" });
                    mqClient.Publish(new Reverse { Value = "World" });
                    mqClient.Publish(new Rot13 { Value = "Orchard" });

                    ExecUtils.RetryOnException(() =>
                    {
                        Assert.That(mqHost.GetStats().TotalMessagesFailed, Is.EqualTo(1 * totalRetries));
                        Assert.That(mqHost.GetStats().TotalMessagesProcessed, Is.EqualTo(2 + 1));
                        Thread.Sleep(100);
                    }, TimeSpan.FromSeconds(5));

                    5.Times(x => mqClient.Publish(new AlwaysThrows { Value = "#" + x }));

                    mqClient.Publish(new Reverse { Value = "Hello" });
                    mqClient.Publish(new Reverse { Value = "World" });
                    mqClient.Publish(new Rot13 { Value = "Orchard" });
                }
                //Debug.WriteLine(mqHost.GetStatsDescription());

                ExecUtils.RetryOnException(() =>
                {
                    Assert.That(mqHost.GetStats().TotalMessagesFailed, Is.EqualTo((1 + 5) * totalRetries));
                    Assert.That(mqHost.GetStats().TotalMessagesProcessed, Is.EqualTo(6));

                    Assert.That(reverseCalled, Is.EqualTo(2 + 2));
                    Assert.That(rot13Called, Is.EqualTo(1 + 1));

                    Thread.Sleep(100);
                }, TimeSpan.FromSeconds(5));
            }
        }

        [Test]
        public void Does_process_messages_sent_before_it_was_started()
        {
            var reverseCalled = 0;

            using (var mqServer = RabbitMqServerTests.CreateMqServer())
            {
                using (var conn = mqServer.ConnectionFactory.CreateConnection())
                using (var channel = conn.CreateModel())
                {
                    channel.PurgeQueue<Reverse>();
                }

                mqServer.RegisterHandler<Reverse>(x => { Interlocked.Increment(ref reverseCalled); return x.GetBody().Value.Reverse(); });

                using (var mqClient = mqServer.CreateMessageQueueClient())
                {
                    RabbitMqServerTests.Publish_4_messages(mqClient);

                    mqServer.Start();

                    ExecUtils.RetryOnException(() =>
                    {
                        Assert.That(mqServer.GetStats().TotalMessagesProcessed, Is.EqualTo(4));
                        Assert.That(reverseCalled, Is.EqualTo(4));
                        Thread.Sleep(100);
                    }, TimeSpan.FromSeconds(5));
                }
            }
        }
    }
}