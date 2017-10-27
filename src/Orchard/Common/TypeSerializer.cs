using Orchard.Reflection.Extensions;
using Orchard.Text;
using Orchard.Utility.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard
{

    /// <summary>
    /// Creates an instance of a Type from a string value
    /// </summary>
    public static class TypeSerializer
    {
        /// <summary>
        /// Print string.Format to Console.WriteLine
        /// </summary>
        public static void Print(this string text, params object[] args)
        {
            if (args.Length > 0)
                Console.WriteLine(text, args);
            else
                Console.WriteLine(text);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="intValue"></param>
        public static void Print(this int intValue)
        {
            Console.WriteLine(intValue.ToString(CultureInfo.InvariantCulture));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="longValue"></param>
        public static void Print(this long longValue)
        {
            Console.WriteLine(longValue.ToString(CultureInfo.InvariantCulture));
        }
        /// <summary>
        /// Recursively prints the contents of any POCO object in a human-friendly, readable format
        /// </summary>
        /// <returns></returns>
        public static string Dump<T>(this T instance)
        {
            return SerializeAndFormat(instance);
        }
        public static string SerializeAndFormat<T>(this T instance)
        {
            var fn = instance as Delegate;
            if (fn != null)
            {
                return Dump(fn);
            }
            var dtoStr = !HasCircularReferences(instance)
                ? instance.ToJson()
                : JsonSerializationHelper.SerializeByDefault(instance.ToSafePartialObjectDictionary());
            return dtoStr;
        }
        public static string Dump(this Delegate fn)
        {
            var method = fn.GetType().GetMethod("Invoke");
            var sb = StringBuilderThreadStatic.Allocate();
            foreach (var param in method.GetParameters())
            {
                if (sb.Length > 0)
                    sb.Append(", ");

                sb.AppendFormat("{0} {1}", param.ParameterType.Name, param.Name);
            }

            var methodName = fn.Method.Name;
            var info = "{0} {1}({2})".Fmt(method.ReturnType.Name, methodName,
                StringBuilderThreadStatic.ReturnAndFree(sb));
            return info;
        }

        public static bool HasCircularReferences(object value)
        {
            return HasCircularReferences(value, null);
        }

        /// <summary>
        /// 循环引用
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parentValues"></param>
        /// <returns></returns>
        private static bool HasCircularReferences(object value, Stack<object> parentValues)
        {
            var type = value?.GetType();

            if (type == null || !type.IsClass || value is string)
                return false;

            if (parentValues == null)
            {
                parentValues = new Stack<object>();
                parentValues.Push(value);
            }

            if (value is IEnumerable valueEnumerable)
            {
                foreach (var item in valueEnumerable)
                {
                    if (HasCircularReferences(item, parentValues))
                        return true;
                }
            }
            else
            {
                var props = type.GetSerializableProperties();

                foreach (var pi in props)
                {
                    if (pi.GetIndexParameters().Length > 0)
                        continue;

                    var mi = pi.PropertyGetMethod();
                    var pValue = mi?.Invoke(value, null);
                    if (pValue == null)
                        continue;

                    if (parentValues.Contains(pValue))
                        return true;

                    parentValues.Push(pValue);

                    if (HasCircularReferences(pValue, parentValues))
                        return true;

                    parentValues.Pop();
                }
            }

            return false;
        }

    }
}
