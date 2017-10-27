using System;
using System.Text;

namespace Orchard.Messaging
{
    /// <summary>
    /// Util static generic class to create unique queue names for types
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class QueueNames<T>
    {
        static QueueNames() 
        {
            Priority = QueueNames.ResolveQueueNameFn(typeof(T).Name, ".priorityq");
            In = QueueNames.ResolveQueueNameFn(typeof(T).Name, ".inq");
            Out = QueueNames.ResolveQueueNameFn(typeof(T).Name, ".outq");
            Dlq = QueueNames.ResolveQueueNameFn(typeof(T).Name, ".dlq");
        }

        public static string Priority { get; private set; }

        public static string In { get; private set; }

        public static string Out { get; private set; }

        public static string Dlq { get; private set; }

        public static string[] AllQueueNames
        {
            get
            {
                return new[] {
                    In,
                    Priority,
                    Out,
                    Dlq,
                };
            }
        }
    }

    /// <summary>
    /// Util class to create unique queue names for runtime types
    /// </summary>
    public class QueueNames
    {
        /// <summary>
        /// Direct Exchange
        /// 处理路由键。需要将一个队列绑定到交换机上，要求该消息与一个特定的路由键完全匹配。这是一个完整的匹配。如果一个队列绑定到该交换机上要求路由键 “dog”，则只有被标记为“dog”的消息才被转发，不会转发dog.puppy，也不会转发dog.guard，只会转发dog。
        /// </summary>
        public static string Exchange = "mx.orchard";
        /// <summary>
        /// Direct Exchange
        /// 处理路由键。需要将一个队列绑定到交换机上，要求该消息与一个特定的路由键完全匹配。这是一个完整的匹配。如果一个队列绑定到该交换机上要求路由键 “dog”，则只有被标记为“dog”的消息才被转发，不会转发dog.puppy，也不会转发dog.guard，只会转发dog。
        /// </summary>
        public static string ExchangeDlq = "mx.orchard.dlq";
        /// <summary>
        /// Topic Exchange
        /// 将路由键和某模式进行匹配。此时队列需要绑定要一个模式上。
        /// 符号“#”匹配一个或多个词，符号“*”匹配不多不少一个词。因此“audit.#”能够匹配到“audit.irs.corporate”，但是“audit.*” 只会匹配到“audit.irs”
        /// </summary>
        public static string ExchangeTopic = "mx.orchard.topic";

        /// <summary>
        /// 
        /// </summary>
        public static string MqPrefix = "mq:";
        public static string QueuePrefix = "";
        public static string TempMqPrefix = MqPrefix + "tmp:";
        public static string TopicIn = MqPrefix + "topic:in";
        public static string TopicOut = MqPrefix + "topic:out";

        public static Func<string, string, string> ResolveQueueNameFn = ResolveQueueName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="queueSuffix"></param>
        /// <returns></returns>
        public static string ResolveQueueName(string typeName, string queueSuffix)
        {
            return QueuePrefix + MqPrefix + typeName + queueSuffix;
        }
        /// <summary>
        /// 是否是临时队列
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns></returns>
        public static bool IsTempQueue(string queueName)
        {
            return queueName != null 
                && queueName.StartsWith(TempMqPrefix, StringComparison.OrdinalIgnoreCase);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefix"></param>
        public static void SetQueuePrefix(string prefix)
        {
            TopicIn = prefix + MqPrefix + "topic:in";
            TopicOut = prefix + MqPrefix + "topic:out";
            QueuePrefix = prefix;
            TempMqPrefix = prefix + MqPrefix + "tmp:";
        }
        /// <summary>
        /// 
        /// </summary>
        private readonly Type messageType;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageType"></param>
        public QueueNames(Type messageType)
        {
            this.messageType = messageType;
        }
        /// <summary>
        /// 
        /// </summary>
        public string Priority
        {
            get { return ResolveQueueNameFn(messageType.Name, ".priorityq"); }
        }
        /// <summary>
        /// 
        /// </summary>
        public string In
        {
            get { return ResolveQueueNameFn(messageType.Name, ".inq"); }
        }
        /// <summary>
        /// 
        /// </summary>

        public string Out
        {
            get { return ResolveQueueNameFn(messageType.Name, ".outq"); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Dlq
        {
            get { return ResolveQueueNameFn(messageType.Name, ".dlq"); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetTempQueueName()
        {
            return TempMqPrefix + Guid.NewGuid().ToString("n");
        }
    }

}