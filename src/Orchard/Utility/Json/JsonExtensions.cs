
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Orchard.Utility.Json
{
    public static class JsonExtensions
    {
        /// <summary>
        /// Converts given object to JSON string.
        /// </summary>
        /// <returns></returns>
        public static string ToJsonString(this object obj, bool camelCase = false, bool indented = false)
        {
            var options = new JsonSerializerSettings();

            if (camelCase)
            {
                options.ContractResolver = new CamelCasePropertyNamesContractResolver();
            }

            if (indented)
            {
                options.Formatting = Formatting.Indented;
            }

            //options.Converters.Insert(0, new AbpDateTimeConverter());

            return JsonConvert.SerializeObject(obj, options);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson<T>(this T obj)
        {
            if (obj == null) return null;
            return ToJsonString(obj);
        }

        public static T FromJson<T>(this string json)
        {
            return JsonSerializationHelper.DeserializeFromString<T>(json);
        }

    }
}
