using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Orchard.Utility;
namespace Orchard
{
    /// <summary>
    /// Extension methods for all objects.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Used to simplify and beautify casting an object to a type. 
        /// </summary>
        /// <typeparam name="T">Type to be casted</typeparam>
        /// <param name="obj">Object to cast</param>
        /// <returns>Casted object</returns>
        public static T As<T>(this object obj)
            where T : class
        {
            return (T)obj;
        }
        /// <summary>
        /// Check if an item is in a list.
        /// </summary>
        /// <param name="item">Item to check</param>
        /// <param name="list">List of items</param>
        /// <typeparam name="T">Type of the items</typeparam>
        public static bool IsIn<T>(this T item, params T[] list)
        {
            return list.Contains(item);
        }
        public static bool IsPrimitive(this Type type)
        {
            if (type == typeof(string))
                return true;
            var typeInfo = type.GetTypeInfo();
            return typeInfo.IsValueType && typeInfo.IsPrimitive;
        }

        private static Func<object, Dictionary<object, object>, object> GetTypeCloner(Type type)
        {
            return _typeCloners.GetOrAdd(type, t => new CloneExpressionBuilder(t).CreateTypeCloner());
        }

        private static readonly ConcurrentDictionary<Type, Func<object, Dictionary<object, object>, object>> _typeCloners = new ConcurrentDictionary<Type, Func<object, Dictionary<object, object>, object>>();
        public static object DeepClone(this object original)
        {
            if (original == null)
                return null;

            var typeToReflect = original.GetType();
            if (IsPrimitive(typeToReflect))
                return original;

            Func<object, Dictionary<object, object>, object> creator = GetTypeCloner(typeToReflect);
            var dict = new Dictionary<object, object>();
            var result = creator(original, dict);
            return result;
        }
        public static T DeepClone<T>(this T original)
        {
            return (T)DeepClone((object)original);
        }
    }
}
