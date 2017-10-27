
using RabbitMQ.Client;
using RabbitMQ.Util;

namespace Orchard.Messaging.RabbitMq
{
    /// <summary>
    /// 
    /// </summary>
    public class RabbitMqBasicConsumer : DefaultBasicConsumer
    {
        /// <summary>
        /// 
        /// </summary>
        readonly SharedQueue<BasicGetResult> queue;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        public RabbitMqBasicConsumer(IModel model) 
            : this(model, new SharedQueue<BasicGetResult>()) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="queue"></param>
        public RabbitMqBasicConsumer(IModel model, SharedQueue<BasicGetResult> queue)
            : base(model)
        {
            this.queue = queue;
        }
        /// <summary>
        /// 
        /// </summary>
        public SharedQueue<BasicGetResult> Queue
        {
            get { return queue; }
        }
        /// <summary>
        /// 
        /// </summary>
        public override void OnCancel()
        {
            queue.Close();
            base.OnCancel();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="consumerTag"></param>
        /// <param name="deliveryTag"></param>
        /// <param name="redelivered"></param>
        /// <param name="exchange"></param>
        /// <param name="routingKey"></param>
        /// <param name="properties"></param>
        /// <param name="body"></param>
        public override void HandleBasicDeliver(
            string consumerTag, ulong deliveryTag, bool redelivered, string exchange,
            string routingKey, IBasicProperties properties, byte[] body)
        {
            var msgResult = new BasicGetResult(
                deliveryTag: deliveryTag,
                redelivered: redelivered,
                exchange: exchange,
                routingKey: routingKey,
                messageCount: 0, //Not available, received by RabbitMQ when declaring queue
                basicProperties: properties,
                body: body);

            queue.Enqueue(msgResult);
        }
    }
}