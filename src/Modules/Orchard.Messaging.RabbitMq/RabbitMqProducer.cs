using Orchard.Client;
using Orchard.Logging;
using Orchard.Utility.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;

namespace Orchard.Messaging.RabbitMq
{
    public class RabbitMqProducer : IMessageProducer, IOneWayClient
    {
        public static ILogger Log = LoggerManager.GetLogger(typeof(RabbitMqProducer));
        protected readonly RabbitMqMessageFactory msgFactory;
        public int RetryCount { get; set; }
        public Action OnPublishedCallback { get; set; }
        public Action<string, IBasicProperties, IMessage> PublishMessageFilter { get; set; }
        public Action<string, BasicGetResult> GetMessageFilter { get; set; }

        private IConnection connection;
        /// <summary>
        /// 
        /// </summary>
        public IConnection Connection
        {
            get
            {
                if (connection == null)
                {
                    connection = msgFactory.ConnectionFactory.CreateConnection();
                }
                return connection;
            }
        }

        private IModel channel;
        /// <summary>
        /// 
        /// </summary>
        public IModel Channel
        {
            get
            {
                if (channel == null || !channel.IsOpen)
                {
                    channel = Connection.OpenChannel();
                    //http://www.rabbitmq.com/blog/2012/04/25/rabbitmq-performance-measurements-part-2/
                    //http://www.rabbitmq.com/amqp-0-9-1-reference.html
                    channel.BasicQos(prefetchCount: 20, prefetchSize: 0, global: false);
                }
                return channel;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msgFactory"></param>
        public RabbitMqProducer(RabbitMqMessageFactory msgFactory)
        {
            this.msgFactory = msgFactory;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="messageBody"></param>
        public virtual void Publish<T>(T messageBody)
        {
            if (messageBody is IMessage message)
            {
                if (message != null)
                {
                    Publish(message.ToInQueueName(), message);
                }
            }
            else
            {
                Publish(new Message<T>(messageBody));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        public virtual void Publish<T>(IMessage<T> message)
        {
            Publish(message.ToInQueueName(), message);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="message"></param>
        public virtual void Publish(string queueName, IMessage message)
        {
            Publish(queueName, message, QueueNames.Exchange);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestDto"></param>
        public virtual void SendOneWay(object requestDto)
        {
            Publish(MessageFactory.Create(requestDto));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="requestDto"></param>
        public virtual void SendOneWay(string queueName, object requestDto)
        {
            Publish(queueName, MessageFactory.Create(requestDto));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requests"></param>
        public virtual void SendAllOneWay(IEnumerable<object> requests)
        {
            if (requests == null) return;
            foreach (var request in requests)
            {
                SendOneWay(request);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="message"></param>
        /// <param name="exchange"></param>
        public virtual void Publish(string queueName, IMessage message, string exchange)
        {
            var props = Channel.CreateBasicProperties();
            props.Persistent = true;
            props.PopulateFromMessage(message);

            if (message.Meta != null)
            {
                props.Headers = new Dictionary<string, object>();
                foreach (var entry in message.Meta)
                {
                    props.Headers[entry.Key] = entry.Value;
                }
            }

            PublishMessageFilter?.Invoke(queueName, props, message);

            var messageBytes = message.Body.ToJson().ToUtf8Bytes();

            PublishMessage(exchange ?? QueueNames.Exchange,
                routingKey: queueName,
                basicProperties: props, body: messageBytes);

            OnPublishedCallback?.Invoke();
        }

        static HashSet<string> Queues = new HashSet<string>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="routingKey"></param>
        /// <param name="basicProperties"></param>
        /// <param name="body"></param>
        public virtual void PublishMessage(string exchange, string routingKey, IBasicProperties basicProperties, byte[] body)
        {
            try
            {
                // In case of server named queues (client declared queue with channel.declare()), assume queue already exists 
                //(redeclaration would result in error anyway since queue was marked as exclusive) and publish to default exchange
                if (routingKey.IsServerNamedQueue()) 
                {
                    Channel.BasicPublish("", routingKey, basicProperties, body);
                }
                else
                {
                    if (!Queues.Contains(routingKey))
                    {
                        Channel.RegisterQueueByName(routingKey);
                        Queues = new HashSet<string>(Queues) { routingKey };
                    }

                    Channel.BasicPublish(exchange, routingKey, basicProperties, body);
                }

            }
            catch (OperationInterruptedException ex)
            {
                if (ex.Is404())
                {
                    // In case of server named queues (client declared queue with channel.declare()), assume queue already exists (redeclaration would result in error anyway since queue was marked as exclusive) and publish to default exchange
                    if (routingKey.IsServerNamedQueue())
                    {
                        Channel.BasicPublish("", routingKey, basicProperties, body);
                    }
                    else
                    {
                        Channel.RegisterExchangeByName(exchange);

                        Channel.BasicPublish(exchange, routingKey, basicProperties, body);
                    }
                }
                throw;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="autoAck"></param>
        /// <returns></returns>
        public virtual BasicGetResult GetMessage(string queueName, bool autoAck)
        {
            try
            {
                if (!Queues.Contains(queueName))
                {
                    Channel.RegisterQueueByName(queueName);
                    Queues = new HashSet<string>(Queues) { queueName };
                }

                var basicMsg = Channel.BasicGet(queueName, autoAck: autoAck);

                GetMessageFilter?.Invoke(queueName, basicMsg);

                return basicMsg;
            }
            catch (OperationInterruptedException ex)
            {
                if (ex.Is404())
                {
                    Channel.RegisterQueueByName(queueName);

                    return Channel.BasicGet(queueName, autoAck: autoAck);
                }
                throw;
            }
        }

        public virtual void Dispose()
        {
            if (channel != null)
            {
                try
                {
                    channel.Dispose();
                }
                catch (Exception ex)
                {
                    Log.Error("Error trying to dispose RabbitMqProducer model", ex);
                } 
                channel = null;
            }
            if (connection != null)
            {
                try
                {
                    connection.Dispose();
                }
                catch (Exception ex)
                {
                    Log.Error("Error trying to dispose RabbitMqProducer connection", ex);
                }
                connection = null;
            }
        }
    }
}