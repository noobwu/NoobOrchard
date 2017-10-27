using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Orchard.Utility.Json
{
    /// <summary>
    /// Defines helper methods to work with JSON.
    /// </summary>
    public static class JsonSerializationHelper
    {
        private const char TypeSeperator = '|';

        /// <summary>
        /// Serializes an object with a type information included.
        /// So, it can be deserialized using <see cref="DeserializeWithType"/> method later.
        /// </summary>
        public static string SerializeWithType(object obj)
        {
            return SerializeWithType(obj, obj.GetType());
        }

        /// <summary>
        /// Serializes an object with a type information included.
        /// So, it can be deserialized using <see cref="DeserializeWithType"/> method later.
        /// </summary>
        public static string SerializeWithType(object obj, Type type)
        {
            var serialized = obj.ToJsonString();

            return string.Format(
                "{0}{1}{2}",
                type.AssemblyQualifiedName,
                TypeSeperator,
                serialized
                );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerializeByDataContract(object obj)
        {
            DataContractJsonSerializerSettings serializerSettings = new DataContractJsonSerializerSettings();
            DataContractJsonSerializer json = new DataContractJsonSerializer(obj.GetType(), serializerSettings);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                json.WriteObject(memoryStream, obj);
                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerializeByDefault(object obj)
        {
            if (obj == null) return null;
            return JsonConvert.SerializeObject(obj, Formatting.None, DefaultSettings());
        }

     
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static JsonSerializerSettings DefaultSettings()
        {
            JsonSerializerSettings jsonSetting = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore   //主要是这一句
            };
            jsonSetting.Converters.Clear();
            jsonSetting.Converters.Add(new IsoDateTimeConverter()
            {
                DateTimeFormat = "yyyy'-'MM'-'dd HH:mm:ss"
            });
            return jsonSetting;
        }
        /// <summary>
        /// Deserializes an object serialized with <see cref="SerializeWithType(object)"/> methods.
        /// </summary>
        public static T DeserializeWithType<T>(string serializedObj)
        {
            return (T)DeserializeWithType(serializedObj);
        }

        /// <summary>
        /// Deserializes an object serialized with <see cref="SerializeWithType(object)"/> methods.
        /// </summary>
        public static object DeserializeWithType(string serializedObj)
        {
            var typeSeperatorIndex = serializedObj.IndexOf(TypeSeperator);
            var type = Type.GetType(serializedObj.Substring(0, typeSeperatorIndex));
            var serialized = serializedObj.Substring(typeSeperatorIndex + 1);
            if (type.IsInterface)
            {
                throw new NotSupportedException("Can not deserialize interface type: "
                    + type.Name);
            }
            else if (type.IsAbstract)
            {
                throw new NotSupportedException("Can not abstract class type: "
                   + type.Name);
            }
            var options = new JsonSerializerSettings();
            //options.Converters.Insert(0, new AbpDateTimeConverter());

            return JsonConvert.DeserializeObject(serialized, type, options);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerializeToString(object obj)
        {
            if (obj == null) return null;
            return JsonConvert.SerializeObject(obj, Formatting.None, DefaultSettings());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T DeserializeFromString<T>(string value)
        {
            var type = typeof(T);
            if (type.IsInterface)
            {
                throw new NotSupportedException("Can not deserialize interface type: "
                    + typeof(T).Name);
            }
            else if (type.IsAbstract)
            {
                throw new NotSupportedException("Can not abstract class type: "
                   + typeof(T).Name);
            }
            if (string.IsNullOrEmpty(value)) return default(T);
            return (T)JsonConvert.DeserializeObject(value, type, DefaultSettings());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strJson"></param>
        /// <returns></returns>
        public static bool ValidateJson(this string strJson)
        {
            if (string.IsNullOrEmpty(strJson)) return false;
            try
            {
                dynamic tmpObj = JsonConvert.DeserializeObject<dynamic>(strJson, DefaultSettings());
                return true;
            }
            catch
            {
                return false;
            }

        }

    }
}
