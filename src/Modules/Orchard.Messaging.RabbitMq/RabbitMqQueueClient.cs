using Orchard.Utility.Json;
using RabbitMQ.Client;
using System;
using System.Threading;

namespace Orchard.Messaging.RabbitMq
{
    public class RabbitMqQueueClient : RabbitMqProducer, IMessageQueueClient
    {
        public RabbitMqQueueClient(RabbitMqMessageFactory msgFactory)
            : base(msgFactory) {}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="message"></param>
        public virtual void Notify(string queueName, IMessage message)
        {
            var json = message.Body.ToJson();
            var messageBytes = json.ToUtf8Bytes();

            PublishMessage(QueueNames.ExchangeTopic,
                routingKey: queueName,
                basicProperties: null, body: messageBytes);
        }
        /// <summary>
        /// 0
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public virtual IMessage<T> Get<T>(string queueName, TimeSpan? timeOut = null)
        {
            var now = DateTime.UtcNow;

            while (timeOut == null || (DateTime.UtcNow - now) < timeOut.Value)
            {
                var basicMsg = GetMessage(queueName, autoAck: false);
                if (basicMsg != null)
                {
                    return basicMsg.ToMessage<T>();
                }
                Thread.Sleep(100);
            }

            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName"></param>
        /// <returns></returns>
        public virtual IMessage<T> GetAsync<T>(string queueName)
        {
            var basicMsg = GetMessage(queueName, autoAck: false);
            return basicMsg.ToMessage<T>();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public virtual void Ack(IMessage message)
        {
            var deliveryTag = ulong.Parse(message.Tag);
            Channel.BasicAck(deliveryTag, multiple:false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="requeue"></param>
        /// <param name="exception"></param>
        public virtual void Nak(IMessage message, bool requeue, Exception exception = null)
        {
            try
            {
                if (requeue)
                {
                    var deliveryTag = ulong.Parse(message.Tag);
                    Channel.BasicNack(deliveryTag, multiple: false, requeue: requeue);
                }
                else
                {
                    Publish(message.ToDlqQueueName(), message, QueueNames.ExchangeDlq);
                    Ack(message);
                }
            }
            catch (Exception)
            {
                var deliveryTag = ulong.Parse(message.Tag);
                Channel.BasicNack(deliveryTag, multiple: false, requeue: requeue);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mqResponse"></param>
        /// <returns></returns>
        public virtual IMessage<T> CreateMessage<T>(object mqResponse)
        {
            var msgResult = mqResponse as BasicGetResult;
            if (msgResult != null)
            {
                return msgResult.ToMessage<T>();
            }

            return (IMessage<T>)mqResponse;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual string GetTempQueueName()
        {
            var anonMq = Channel.QueueDeclare(
                queue: QueueNames.GetTempQueueName(),
                durable:false,
                exclusive:true,
                autoDelete:true,
                arguments:null);

            return anonMq.QueueName;
        }
    }
}