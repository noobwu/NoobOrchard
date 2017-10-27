using System;
using System.Collections.Generic;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Orchard.Environment;
using Orchard.Utility.Json;
using Orchard.Domain.Entities;
using Orchard.Client;
using Orchard.FileSystems;

namespace Orchard.Messaging.RabbitMq
{
    /// <summary>
    /// 
    /// </summary>
    public static class RabbitMqExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static IModel OpenChannel(this IConnection connection)
        {
            var channel = connection.CreateModel();
            channel.RegisterDirectExchange();
            channel.RegisterDlqExchange();
            channel.RegisterTopicExchange();
            return channel;
        }
        /// <summary>
        /// Direct Exchange
        /// 处理路由键。需要将一个队列绑定到交换机上，要求该消息与一个特定的路由键完全匹配。这是一个完整的匹配。如果一个队列绑定到该交换机上要求路由键 “dog”，则只有被标记为“dog”的消息才被转发，不会转发dog.puppy，也不会转发dog.guard，只会转发dog。 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="exchangeName"></param>
        public static void RegisterDirectExchange(this IModel channel, string exchangeName = null)
        {
            channel.ExchangeDeclare(exchangeName ?? QueueNames.Exchange, "direct", durable: true, autoDelete: false, arguments: null);
        }
        /// <summary>
        /// Direct Exchange
        /// 处理路由键。需要将一个队列绑定到交换机上，要求该消息与一个特定的路由键完全匹配。这是一个完整的匹配。如果一个队列绑定到该交换机上要求路由键 “dog”，则只有被标记为“dog”的消息才被转发，不会转发dog.puppy，也不会转发dog.guard，只会转发dog。 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="exchangeName"></param>
        public static void RegisterDlqExchange(this IModel channel, string exchangeName = null)
        {
            channel.ExchangeDeclare(exchangeName ?? QueueNames.ExchangeDlq, "direct", durable: true, autoDelete: false, arguments:null);
        }
        /// <summary>
        ///  Topic Exchange
        ///  将路由键和某模式进行匹配。此时队列需要绑定要一个模式上。符号“#”匹配一个或多个词，符号“*”匹配不多不少一个词。因此“audit.#”能够匹配到“audit.irs.corporate”，但是“audit.*” 只会匹配到“audit.irs”
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="exchangeName"></param>
        public static void RegisterTopicExchange(this IModel channel, string exchangeName = null)
        {
            channel.ExchangeDeclare(exchangeName ?? QueueNames.ExchangeTopic, "topic", durable: false, autoDelete: false, arguments: null);
        }
        /// <summary>
        /// Fanout Exchange
        /// 不处理路由键。你只需要简单的将队列绑定到交换机上。一个发送到交换机的消息都会被转发到与该交换机绑定的所有队列上。很像子网广播，每台子网内的主机都获得了一份复制的消息。Fanout交换机转发消息是最快的。 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="exchangeName"></param>
        public static void RegisterFanoutExchange(this IModel channel, string exchangeName)
        {
            //声明fanout类型的Exchange
            channel.ExchangeDeclare(exchangeName, "fanout", durable: false, autoDelete: false, arguments: null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        public static void RegisterQueues<T>(this IModel channel)
        {
            channel.RegisterQueue(QueueNames<T>.In);
            channel.RegisterQueue(QueueNames<T>.Priority);
            channel.RegisterTopic(QueueNames<T>.Out);
            channel.RegisterDlq(QueueNames<T>.Dlq);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="queueNames"></param>
        public static void RegisterQueues(this IModel channel, QueueNames queueNames)
        {
            channel.RegisterQueue(queueNames.In);
            channel.RegisterQueue(queueNames.Priority);
            channel.RegisterTopic(queueNames.Out);
            channel.RegisterDlq(queueNames.Dlq);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static RabbitMqServer GetRabbitMqServer()
        {
            if (HostContext.AppHost == null)
                return null;

            return HostContext.TryResolve<IMessageService>() as RabbitMqServer;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="queueName"></param>
        public static void RegisterQueue(this IModel channel, string queueName)
        {
            var args = new Dictionary<string, object> {
                {"x-dead-letter-exchange", QueueNames.ExchangeDlq },//
                {"x-dead-letter-routing-key", queueName.Replace(".inq",".dlq").Replace(".priorityq",".dlq") },
            };

            GetRabbitMqServer()?.CreateQueueFilter?.Invoke(queueName, args);

            if (!QueueNames.IsTempQueue(queueName)) //Already declared in GetTempQueueName()
            {
                //声明队列
                channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, arguments: args);
            }
            //绑定队列到指定的Exchange
            channel.QueueBind(queueName, QueueNames.Exchange, routingKey: queueName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="queueName"></param>
        public static void RegisterDlq(this IModel channel, string queueName)
        {
            var args = new Dictionary<string, object>();

            GetRabbitMqServer()?.CreateQueueFilter?.Invoke(queueName, args);
            //声明队列
            channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, arguments: args);
            //绑定队列到指定的Exchange
            channel.QueueBind(queueName, QueueNames.ExchangeDlq, routingKey: queueName);
        }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="channel"></param>
       /// <param name="queueName"></param>
        public static void RegisterTopic(this IModel channel, string queueName)
        {
            var args = new Dictionary<string, object>();

            GetRabbitMqServer()?.CreateTopicFilter?.Invoke(queueName, args);
            //声明队列
            channel.QueueDeclare(queueName, durable: false, exclusive: false, autoDelete: false, arguments: args);
            //绑定队列到指定的Exchange
            channel.QueueBind(queueName, QueueNames.ExchangeTopic, routingKey: queueName);
        }
        /// <summary>
        /// 删除队列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        public static void DeleteQueue<T>(this IModel model)
        {
            model.DeleteQueues(QueueNames<T>.AllQueueNames);
        }
        /// <summary>
        /// 批量删除指定的队列列表
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="queues"></param>
        public static void DeleteQueues(this IModel channel, params string[] queues)
        {
            foreach (var queue in queues)
            {
                try
                {
                    channel.QueueDelete(queue, ifUnused:false, ifEmpty:false);
                }
                catch (OperationInterruptedException ex)
                {
                    if (!ex.Message.Contains("code=404"))
                        throw;
                }
            }
        }
        /// <summary>
        /// 清除消息队列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        public static void PurgeQueue<T>(this IModel model)
        {
            model.PurgeQueues(QueueNames<T>.AllQueueNames);
        }
        /// <summary>
        /// 清除消息队列
        /// </summary>
        /// <param name="model"></param>
        /// <param name="queues"></param>
        public static void PurgeQueues(this IModel model, params string[] queues)
        {
            foreach (var queue in queues)
            {
                try
                {
                    model.QueuePurge(queue);
                }
                catch (OperationInterruptedException ex)
                {
                    if (!ex.Is404())
                        throw;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="exchange"></param>
        public static void RegisterExchangeByName(this IModel channel, string exchange)
        {
            if (exchange.EndsWith(".dlq"))
                channel.RegisterDlqExchange(exchange);
            else if (exchange.EndsWith(".topic"))
                channel.RegisterTopicExchange(exchange);
            else 
                channel.RegisterDirectExchange(exchange);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="queueName"></param>
        public static void RegisterQueueByName(this IModel channel, string queueName)
        {
            if (queueName.EndsWith(".dlq"))
                channel.RegisterDlq(queueName);
            else if (queueName.EndsWith(".outq"))
                channel.RegisterTopic(queueName);
            else
                channel.RegisterQueue(queueName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        internal static bool Is404(this OperationInterruptedException ex)
        {
            return ex.Message.Contains("code=404");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        public static bool IsServerNamedQueue(this string queueName)
        {
            if (string.IsNullOrEmpty(queueName))
            {
                throw new ArgumentNullException("queueName");
            }

            var lowerCaseQueue = queueName.ToLower();
            return lowerCaseQueue.StartsWith("amq.")
                || lowerCaseQueue.StartsWith(QueueNames.TempMqPrefix);
        }	
        /// <summary>
        /// 
        /// </summary>
        /// <param name="props"></param>
        /// <param name="message"></param>
        public static void PopulateFromMessage(this IBasicProperties props, IMessage message)
        {
            props.MessageId = message.Id.ToString();
            props.Timestamp = new AmqpTimestamp(message.CreatedDate.ToUnixTime());
            props.Priority = (byte)message.Priority;
            props.ContentType = MimeTypes.Json;
            
            if (message.Body != null)
            {
                props.Type = message.Body.GetType().Name;
            }

            if (message.ReplyTo != null)
            {
                props.ReplyTo = message.ReplyTo;
            }

            if (message.ReplyId != null)
            {
                props.CorrelationId = message.ReplyId.Value.ToString();
            }

            if (message.Error != null)
            {
                if (props.Headers == null)
                    props.Headers = new Dictionary<string, object>();
                props.Headers["Error"] = message.Error.ToJson();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msgResult"></param>
        /// <returns></returns>
        public static IMessage<T> ToMessage<T>(this BasicGetResult msgResult)
        {
            if (msgResult == null)
                return null;

            var props = msgResult.BasicProperties;
            T body;

            if (string.IsNullOrEmpty(props.ContentType) || props.ContentType.MatchesContentType(MimeTypes.Json))
            {
                var json = msgResult.Body.FromUtf8Bytes();
                body = json.FromJson<T>();
            }
            else
            {
                var deserializer = HostContext.ContentTypes.GetStreamDeserializer(props.ContentType);
                if (deserializer == null)
                    throw new NotSupportedException("Unknown Content-Type: " + props.ContentType);
                var ms = MemoryStreamFactory.GetStream(msgResult.Body);
                body = (T)deserializer(typeof(T), ms);
                ms.Dispose();
            }

            var message = new Message<T>(body)
            {
                Id = props.MessageId != null ? Guid.Parse(props.MessageId) : new Guid(),
                CreatedDate = ((int) props.Timestamp.UnixTime).FromUnixTime(),
                Priority = props.Priority,
                ReplyTo = props.ReplyTo,
                Tag = msgResult.DeliveryTag.ToString(),
                RetryAttempts = msgResult.Redelivered ? 1 : 0,
            };

            if (props.CorrelationId != null)
            {
                message.ReplyId = Guid.Parse(props.CorrelationId);
            }

            if (props.Headers != null)
            {
                foreach (var entry in props.Headers)
                {
                    if (entry.Key == "Error")
                    {
                        var errors = entry.Value;
                        if (errors != null)
                        {
                            var errorBytes = errors as byte[];
                            var errorsJson = errorBytes != null
                                ? errorBytes.FromUtf8Bytes()
                                : errors.ToString();
                            message.Error = errorsJson.FromJson<ResponseStatus>();
                        }
                    }
                    else
                    {
                        if (message.Meta == null)
                            message.Meta = new Dictionary<string, string>();

                        var bytes = entry.Value as byte[];
                        var value = bytes != null
                            ? bytes.FromUtf8Bytes()
                            : entry.Value?.ToString();

                        message.Meta[entry.Key] = value;
                    }
                }
            }

            return message;
        }
    }
}