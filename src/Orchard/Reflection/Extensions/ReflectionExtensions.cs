using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Reflection.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class ReflectionExtensions
    {
        private static readonly Dictionary<Type, TypeCode> _typeCodeTable =
      new Dictionary<Type, TypeCode>
      {
            { typeof(Boolean), TypeCode.Boolean },
            { typeof(Char), TypeCode.Char },
            { typeof(Byte), TypeCode.Byte },
            { typeof(Int16), TypeCode.Int16 },
            { typeof(Int32), TypeCode.Int32 },
            { typeof(Int64), TypeCode.Int64 },
            { typeof(SByte), TypeCode.SByte },
            { typeof(UInt16), TypeCode.UInt16 },
            { typeof(UInt32), TypeCode.UInt32 },
            { typeof(UInt64), TypeCode.UInt64 },
            { typeof(Single), TypeCode.Single },
            { typeof(Double), TypeCode.Double },
            { typeof(DateTime), TypeCode.DateTime },
            { typeof(Decimal), TypeCode.Decimal },
            { typeof(String), TypeCode.String },
      };


        public static TypeCode GetTypeCode(this Type type)
        {
#if (NETFX_CORE || PCL || NETSTANDARD1_1)
            if (type == null)
            {
                return TypeCode.Empty;
            }

            TypeCode result;
            if (!_typeCodeTable.TryGetValue(type, out result))
            {
                result = TypeCode.Object;
            }

            return result;
#else
            return Type.GetTypeCode(type);
#endif
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="thisOrBaseType"></param>
        /// <returns></returns>
        public static bool IsInstanceOf(this Type type, Type thisOrBaseType)
        {
            while (type != null)
            {
                if (type == thisOrBaseType)
                    return true;

                type = type.BaseType();
            }

            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="genericTypeDefinition"></param>
        /// <returns></returns>
        public static Type GetTypeWithGenericTypeDefinitionOf(this Type type, Type genericTypeDefinition)
        {
            foreach (var t in type.GetTypeInterfaces())
            {
                if (t.IsGenericType && t.GetGenericTypeDefinition() == genericTypeDefinition)
                {
                    return t;
                }
            }

            var genericType = type.FirstGenericType();
            if (genericType != null && genericType.GetGenericTypeDefinition() == genericTypeDefinition)
            {
                return genericType;
            }

            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type FirstGenericType(this Type type)
        {
            while (type != null)
            {
                if (type.IsGenericType)
                    return type;

                type = type.BaseType();
            }
            return null;
        }
        public static PropertyInfo[] GetSerializableProperties(this Type type)
        {
            var properties = type.IsDto()
                ? type.GetAllProperties()
                : type.GetPublicProperties();
            return properties.OnlySerializableProperties(type);
        }

        public static PropertyInfo[] OnlySerializableProperties(this PropertyInfo[] properties, Type type = null)
        {
            var isDto = type.IsDto();
            var readableProperties = properties.Where(x => x.PropertyGetMethod(nonPublic: isDto) != null);

            if (isDto)
            {
                return readableProperties.Where(attr =>
                    attr.HasAttribute<DataMemberAttribute>()).ToArray();
            }

            // else return those properties that are not decorated with IgnoreDataMember
            return readableProperties
                .Where(prop => prop.AllAttributes()
                    .All(attr =>
                    {
                        var name = attr.GetType().Name;
                        return !IgnoreAttributesNamed.Contains(name);
                    }))
                .ToArray();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static PropertyInfo[] GetPublicProperties(this Type type)
        {
            if (type.IsInterface)
            {
                var propertyInfos = new List<PropertyInfo>();

                var considered = new List<Type>();
                var queue = new Queue<Type>();
                considered.Add(type);
                queue.Enqueue(type);

                while (queue.Count > 0)
                {
                    var subType = queue.Dequeue();
                    foreach (var subInterface in subType.GetTypeInterfaces())
                    {
                        if (considered.Contains(subInterface)) continue;

                        considered.Add(subInterface);
                        queue.Enqueue(subInterface);
                    }

                    var typeProperties = subType.GetTypesPublicProperties();

                    var newPropertyInfos = typeProperties
                        .Where(x => !propertyInfos.Contains(x));

                    propertyInfos.InsertRange(0, newPropertyInfos);
                }

                return propertyInfos.ToArray();
            }

            return type.GetTypesPublicProperties()
                .Where(t => t.GetIndexParameters().Length == 0) // ignore indexed properties
                .ToArray();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static PropertyInfo[] GetAllProperties(this Type type)
        {
            if (type.IsInterface)
            {
                var propertyInfos = new List<PropertyInfo>();

                var considered = new List<Type>();
                var queue = new Queue<Type>();
                considered.Add(type);
                queue.Enqueue(type);

                while (queue.Count > 0)
                {
                    var subType = queue.Dequeue();
                    foreach (var subInterface in subType.GetTypeInterfaces())
                    {
                        if (considered.Contains(subInterface)) continue;

                        considered.Add(subInterface);
                        queue.Enqueue(subInterface);
                    }

                    var typeProperties = subType.GetTypesProperties();

                    var newPropertyInfos = typeProperties
                        .Where(x => !propertyInfos.Contains(x));

                    propertyInfos.InsertRange(0, newPropertyInfos);
                }

                return propertyInfos.ToArray();
            }

            return type.GetTypesProperties()
                .Where(t => t.GetIndexParameters().Length == 0) // ignore indexed properties
                .ToArray();
        }
        public const string DataMember = "DataMemberAttribute";

        internal static string[] IgnoreAttributesNamed = new[] {
            "IgnoreDataMemberAttribute",
            "JsonIgnoreAttribute"
        };

        internal static void Reset()
        {
            IgnoreAttributesNamed = new[] {
                "IgnoreDataMemberAttribute",
                "JsonIgnoreAttribute"
            };
        }
    }
}