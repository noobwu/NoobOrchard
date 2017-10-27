using Newtonsoft.Json.Linq;
using Orleans.Runtime;
using Orleans.Serialization;
using System;

namespace NoobOrleans.Core
{
    /// <summary>
    /// An implementation of IExternalSerializer for usage with linq types.
    /// </summary>
    public  class JsonSerializer : IExternalSerializer
    {
        //增加自定义的处理类
        private static readonly Type JsonType = typeof(JObject);
        protected Logger logger;
        public JsonSerializer()
        {
            
        }
        /// <summary>
        /// Initializes the external serializer
        /// </summary>
        /// <param name="logger">The logger to use to capture any serialization events</param>
        public void Initialize(Logger logger)
        {
            this.logger = logger;
        }
        /// <summary>
        /// Informs the serialization manager whether this serializer supports the type for serialization.
        /// </summary>
        /// <param name="itemType">The type of the item to be serialized</param>
        /// <returns>A value indicating whether the item can be serialized.</returns>
        public bool IsSupportedType(Type itemType)
        {
            if (JsonType.IsAssignableFrom(itemType))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Tries to create a copy of source.
        /// </summary>
        /// <param name="source">The item to create a copy of</param>
        /// <param name="context">The context in which the object is being copied.</param>
        /// <returns>The copy</returns>
        public virtual object DeepCopy(object source, ICopyContext context)
        {
            if (source == null)
            {
                return null;
            }
            //Expression expression = source as Expression;
            //if (expression == null)
            //{
            //    throw new ArgumentException("The provided item for serialization in not an instance of " + typeof(Expression), "item");
            //}
            //byte[] outBytes = serializer.SerializeBinary(expression);
            //object target = serializer.DeserializeBinary(outBytes);//
            //return target;
            return source;
        }
        /// <summary>
        /// Tries to serialize an item.
        /// </summary>
        /// <param name="item">The instance of the object being serialized</param>
        /// <param name="context">The context in which the object is being serialized.</param>
        /// <param name="expectedType">The type that the deserializer will expect</param>
        public virtual void Serialize(object item, ISerializationContext context, Type expectedType)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            var writer = context.StreamWriter;
            if (item == null)
            {
                writer.WriteNull();
                return;
            }
            Type type = item.GetType();
            var typeHandle = type.TypeHandle;
            if (logger != null)
            {
                logger.Verbose("JsonSerializer,Serialize,expectedType:" + expectedType + ",item.type：" + type + ",item.TypeHandle:" + typeHandle);
            }
            var input = item as JObject;
            string str = input.ToString();
            if (logger != null)
            {
                logger.Verbose("JsonSerializer,JObject.ToString():" + str);
            }
            context.SerializationManager.Serialize(str, context.StreamWriter);
        }
        /// <summary>
        /// Tries to deserialize an item.
        /// </summary>
        /// <param name="context">The context in which the object is being deserialized.</param>
        /// <param name="expectedType">The type that should be deserialized</param>
        /// <returns>The deserialized object</returns>
        public virtual object Deserialize(Type expectedType, IDeserializationContext context)
        {
            if (expectedType == null)
            {
                throw new ArgumentNullException(nameof(expectedType));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            var typeHandle = expectedType.TypeHandle;
            if (logger != null)
            {
                logger.Verbose("JsonSerializer,Deserialize,expectedType：" + expectedType + ",expectedType.TypeHandle:" + expectedType.TypeHandle);
            }
            var str = (string)context.SerializationManager.Deserialize(typeof(string), context.StreamReader);
            if (logger != null)
            {
                logger.Verbose("JsonSerializer,Deserialize,str：" + str);
            }
            return JObject.Parse(str);
        }
    }
}
