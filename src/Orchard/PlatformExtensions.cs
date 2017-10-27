using Orchard.Reflection.Extensions;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Orchard
{
    /// <summary>
    /// 
    /// </summary>
    public static class PlatformExtensions
    {
        #region  variable
        //Should only register Runtime Attributes on StartUp, So using non-ThreadSafe Dictionary is OK
        static Dictionary<string, List<Attribute>> propertyAttributesMap
            = new Dictionary<string, List<Attribute>>();

        static Dictionary<Type, List<Attribute>> typeAttributesMap
        = new Dictionary<Type, List<Attribute>>();
        const string DataContract = "DataContractAttribute";

        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InstanceOfType(this Type type, object instance)
        {
#if (NETFX_CORE || PCL || NETSTANDARD1_1)
            var result = instance != null && type.GetTypeInfo().IsAssignableFrom(instance.GetType().GetTypeInfo()); //https://stackoverflow.com/a/24712250/85785
            return result;
#else
            var result = type.IsInstanceOfType(instance);
            return result;
#endif
        }
        public static void ClearRuntimeAttributes()
        {
            propertyAttributesMap = new Dictionary<string, List<Attribute>>();
            typeAttributesMap = new Dictionary<Type, List<Attribute>>();
        }

        internal static string UniqueKey(this PropertyInfo pi)
        {
            if (pi.DeclaringType == null)
                throw new ArgumentException("Property '{0}' has no DeclaringType".Fmt(pi.Name));

            return pi.DeclaringType.Namespace + "." + pi.DeclaringType.Name + "." + pi.Name;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MethodInfo GetStaticMethod(this Type type, string methodName)
        {
#if (NETFX_CORE || PCL || NETSTANDARD1_1)
            return type.GetMethodInfo(methodName);
#else
            return type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Delegate MakeDelegate(this MethodInfo mi, Type delegateType, bool throwOnBindFailure = true)
        {
#if (NETFX_CORE || PCL || NETSTANDARD1_1)
            return mi.CreateDelegate(delegateType);
#else
            return Delegate.CreateDelegate(delegateType, mi, throwOnBindFailure);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static IEnumerable<T> GetRuntimeAttributes<T>(this Type type)
        {
            return typeAttributesMap.TryGetValue(type, out List<Attribute> attrs)
                ? attrs.OfType<T>()
                : new List<T>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static IEnumerable<Attribute> GetRuntimeAttributes(this Type type, Type attrType = null)
        {
            return typeAttributesMap.TryGetValue(type, out List<Attribute> attrs)
                ? attrs.Where(x => attrType == null || attrType.IsInstanceOf(x.GetType()))
                : new List<Attribute>();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TAttr FirstAttribute<TAttr>(this Type type) where TAttr : class
        {
#if (NETFX_CORE || PCL )

            return (TAttr)type.GetTypeInfo().GetCustomAttributes(typeof(TAttr), true)
                    .Cast<TAttr>()
                    .FirstOrDefault();
#elif NETSTANDARD1_1                   
            return (TAttr)type.GetTypeInfo().GetCustomAttributes(typeof(TAttr), true)
                    .Cast<TAttr>()
                    .FirstOrDefault()
                   ?? type.GetRuntimeAttributes<TAttr>().FirstOrDefault();
#else
            return (TAttr)type.GetCustomAttributes(typeof(TAttr), true)
                       .FirstOrDefault()
                   ?? type.GetRuntimeAttributes<TAttr>().FirstOrDefault();
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type BaseType(this Type type)
        {
#if (NETFX_CORE || PCL || NETSTANDARD1_1)
            return type.GetTypeInfo().BaseType;
#else
            return type.BaseType;
#endif
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TAttr[] AllAttributes<TAttr>(this Type type)
#if (NETFX_CORE || PCL || NETSTANDARD1_1)
            where TAttr : Attribute
#endif
        {
#if (NETFX_CORE || PCL)
            return type.GetTypeInfo().GetCustomAttributes<TAttr>(true).ToArray();
#elif NETSTANDARD1_1
            return type.GetTypeInfo().GetCustomAttributes<TAttr>(true)
                .Union(type.GetRuntimeAttributes<TAttr>())
                .ToArray();
#else
            return type.GetCustomAttributes(typeof(TAttr), true)
                .OfType<TAttr>()
                .Union(type.GetRuntimeAttributes<TAttr>())
                .ToArray();
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object[] AllAttributes(this ParameterInfo paramInfo, Type attrType)
        {
#if (NETFX_CORE || PCL || NETSTANDARD1_1)
            return paramInfo.GetCustomAttributes(true).Where(x => x.GetType().IsInstanceOf(attrType)).ToArray();
#else
            return paramInfo.GetCustomAttributes(attrType, true);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object[] AllAttributes(this MemberInfo memberInfo, Type attrType)
        {
#if (NETFX_CORE || PCL || NETSTANDARD1_1)
            return memberInfo.GetCustomAttributes(true).Where(x => x.GetType().IsInstanceOf(attrType)).ToArray();
#else
            if (memberInfo is PropertyInfo prop)
                return prop.AllAttributes(attrType);

            return memberInfo.GetCustomAttributes(attrType, true);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object[] AllAttributes(this FieldInfo fieldInfo, Type attrType)
        {
#if (NETFX_CORE || PCL || NETSTANDARD1_1)
            return fieldInfo.GetCustomAttributes(true).Where(x => x.GetType().IsInstanceOf(attrType)).ToArray();
#else
            return fieldInfo.GetCustomAttributes(attrType, true);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TAttr[] AllAttributes<TAttr>(this ParameterInfo pi)
        {
            return pi.AllAttributes(typeof(TAttr)).Cast<TAttr>().ToArray();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TAttr[] AllAttributes<TAttr>(this MemberInfo mi)
        {
            return mi.AllAttributes(typeof(TAttr)).Cast<TAttr>().ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TAttr[] AllAttributes<TAttr>(this FieldInfo fi)
        {
            return fi.AllAttributes(typeof(TAttr)).Cast<TAttr>().ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TAttr[] AllAttributes<TAttr>(this PropertyInfo pi)
        {
            return pi.AllAttributes(typeof(TAttr)).Cast<TAttr>().ToArray();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TAttribute FirstAttribute<TAttribute>(this MemberInfo memberInfo)
        {
            return memberInfo.AllAttributes<TAttribute>().FirstOrDefault();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TAttribute FirstAttribute<TAttribute>(this ParameterInfo paramInfo)
        {
            return paramInfo.AllAttributes<TAttribute>().FirstOrDefault();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TAttribute FirstAttribute<TAttribute>(this PropertyInfo propertyInfo)
        {
            return propertyInfo.AllAttributes<TAttribute>().FirstOrDefault();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PropertyInfo GetPropertyInfo(this Type type, string propertyName)
        {
#if (NETFX_CORE || PCL || NETSTANDARD1_1)
            return type.GetRuntimeProperty(propertyName);
#else
            return type.GetProperty(propertyName);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MethodInfo GetMethodInfo(this PropertyInfo pi, bool nonPublic = true)
        {
#if (NETFX_CORE || PCL || NETSTANDARD1_1)
            return pi.GetMethod;
#else
            return pi.GetGetMethod(nonPublic);
#endif
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type[] GetTypeInterfaces(this Type type)
        {
#if (NETFX_CORE || PCL || NETSTANDARD1_1)
            return type.GetTypeInfo().ImplementedInterfaces.ToArray();
#else
            return type.GetInterfaces();
#endif
        }
       


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDto(this Type type)
        {
            if (type == null)
                return false;

#if (NETFX_CORE || PCL || NETSTANDARD1_1)
            return type.HasAttribute<DataContractAttribute>();
#else
            return type.HasAttribute<DataContractAttribute>();
#endif
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static PropertyInfo[] GetTypesPublicProperties(this Type subType)
        {
#if (NETFX_CORE || PCL || NETSTANDARD1_1)
            var pis = new List<PropertyInfo>();
            foreach (var pi in subType.GetRuntimeProperties())
            {
                var mi = pi.GetMethod ?? pi.SetMethod;
                if (mi != null && mi.IsStatic) continue;
                pis.Add(pi);
            }
            return pis.ToArray();
#else
            return subType.GetProperties(
                BindingFlags.FlattenHierarchy |
                BindingFlags.Public |
                BindingFlags.Instance);
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static PropertyInfo[] GetTypesProperties(this Type subType)
        {
#if (NETFX_CORE || PCL || NETSTANDARD1_1)
            var pis = new List<PropertyInfo>();
            foreach (var pi in subType.GetRuntimeProperties())
            {
                var mi = pi.GetMethod ?? pi.SetMethod;
                if (mi != null && mi.IsStatic) continue;
                pis.Add(pi);
            }
            return pis.ToArray();
#else
            return subType.GetProperties(
                BindingFlags.FlattenHierarchy |
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance);
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MethodInfo PropertyGetMethod(this PropertyInfo pi, bool nonPublic = false)
        {
#if (NETFX_CORE || PCL || NETSTANDARD1_1)
            var mi = pi.GetMethod;
            return mi != null && (nonPublic || mi.IsPublic) ? mi : null;
#else
            return pi.GetGetMethod(nonPublic);
#endif
        }


        #region Attributes

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object[] AllAttributes(this Type type)
        {
#if (NETFX_CORE || PCL)
            return type.GetTypeInfo().GetCustomAttributes(true).ToArray();
#elif NETSTANDARD1_1
            return type.GetTypeInfo().GetCustomAttributes(true).Union(type.GetRuntimeAttributes()).ToArray();
#else
            return type.GetCustomAttributes(true).Union(type.GetRuntimeAttributes()).ToArray();
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAttribute<T>(this Type type)
        {
            return type.AllAttributes().Any(x => x.GetType() == typeof(T));
        }
        public static object[] AllAttributes(this PropertyInfo propertyInfo)
        {
#if (NETFX_CORE || PCL)
            return propertyInfo.GetCustomAttributes(true).ToArray();
#else
#if NETSTANDARD1_1
            var attrs = propertyInfo.GetCustomAttributes(true).ToArray();
#else
            var attrs = propertyInfo.GetCustomAttributes(true);
#endif
            var runtimeAttrs = propertyInfo.GetAttributes();
            if (runtimeAttrs.Count == 0)
                return attrs;

            runtimeAttrs.AddRange(attrs.Cast<Attribute>());
            return runtimeAttrs.Cast<object>().ToArray();
#endif
        }
        public static List<Attribute> GetAttributes(this PropertyInfo propertyInfo, Type attrType)
        {
            return !propertyAttributesMap.TryGetValue(propertyInfo.UniqueKey(), out List<Attribute> propertyAttrs)
                ? new List<Attribute>()
                : propertyAttrs.Where(x => attrType.IsInstanceOf(x.GetType())).ToList();
        }
        public static object[] AllAttributes(this PropertyInfo propertyInfo, Type attrType)
        {
#if (NETFX_CORE || PCL)
            return propertyInfo.GetCustomAttributes(true).Where(x => x.GetType().IsInstanceOf(attrType)).ToArray();
#else
#if NETSTANDARD1_1
            var attrs = propertyInfo.GetCustomAttributes(attrType, true).ToArray();
#else
            var attrs = propertyInfo.GetCustomAttributes(attrType, true);
#endif
            var runtimeAttrs = propertyInfo.GetAttributes(attrType);
            if (runtimeAttrs.Count == 0)
                return attrs;

            runtimeAttrs.AddRange(attrs.Cast<Attribute>());
            return runtimeAttrs.Cast<object>().ToArray();
#endif
        }

        public static List<TAttr> GetAttributes<TAttr>(this PropertyInfo propertyInfo)
        {
            return !propertyAttributesMap.TryGetValue(propertyInfo.UniqueKey(), out List<Attribute> propertyAttrs)
                ? new List<TAttr>()
                : propertyAttrs.OfType<TAttr>().ToList();
        }

        public static List<Attribute> GetAttributes(this PropertyInfo propertyInfo)
        {
            return !propertyAttributesMap.TryGetValue(propertyInfo.UniqueKey(), out List<Attribute> propertyAttrs)
                ? new List<Attribute>()
                : propertyAttrs.ToList();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object[] AllAttributes(this ParameterInfo paramInfo)
        {
#if (NETFX_CORE || PCL || NETSTANDARD1_1)
            return paramInfo.GetCustomAttributes(true).ToArray();
#else
            return paramInfo.GetCustomAttributes(true);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object[] AllAttributes(this FieldInfo fieldInfo)
        {
#if (NETFX_CORE || PCL || NETSTANDARD1_1)
            return fieldInfo.GetCustomAttributes(true).ToArray();
#else
            return fieldInfo.GetCustomAttributes(true);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object[] AllAttributes(this MemberInfo memberInfo)
        {
#if (NETFX_CORE || PCL || NETSTANDARD1_1)
            return memberInfo.GetCustomAttributes(true).ToArray();
#else
            return memberInfo.GetCustomAttributes(true);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object[] AllAttributes(this Type type, Type attrType)
        {
#if (NETFX_CORE || PCL)
            return type.GetTypeInfo().GetCustomAttributes(true).Where(x => x.GetType().IsInstanceOf(attrType)).ToArray();
#elif NETSTANDARD1_1
            return type.GetTypeInfo().GetCustomAttributes(attrType, true)
                .Union(type.GetRuntimeAttributes(attrType))
                .ToArray();
#else
            return type.GetCustomAttributes(attrType, true).Union(type.GetRuntimeAttributes(attrType)).ToArray();
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object[] AllAttributes(this Assembly assembly)
        {
#if (NETFX_CORE || PCL || NETSTANDARD1_1)
            return assembly.GetCustomAttributes().ToArray();
#else
            return assembly.GetCustomAttributes(true).ToArray();
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAttribute<T>(this PropertyInfo pi)
        {
            return pi.AllAttributes().Any(x => x.GetType() == typeof(T));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAttribute<T>(this FieldInfo fi)
        {
            return fi.AllAttributes().Any(x => x.GetType() == typeof(T));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasAttribute<T>(this MethodInfo mi)
        {
            return mi.AllAttributes().Any(x => x.GetType() == typeof(T));
        }
        #endregion Attributes
        public static Dictionary<string, object> ToObjectDictionary(this object obj)
        {
            if (obj is Dictionary<string, object> alreadyDict)
                return alreadyDict;

            if (obj is IDictionary<string, object> interfaceDict)
                return new Dictionary<string, object>(interfaceDict);

            if (obj is Dictionary<string, string> stringDict)
            {
                var to = new Dictionary<string, object>();
                foreach (var entry in stringDict)
                {
                    to[entry.Key] = entry.Value;
                }
                return to;
            }

            return null;
        }
        public static Dictionary<string, object> ToSafePartialObjectDictionary<T>(this T instance)
        {
            var to = new Dictionary<string, object>();
            var propValues = instance.ToObjectDictionary();
            if (propValues != null)
            {
                foreach (var entry in propValues)
                {
                    var valueType = entry.Value?.GetType();

                    if (valueType == null || !valueType.IsClass || valueType == typeof(string))
                    {
                        to[entry.Key] = entry.Value;
                    }
                    else if (!TypeSerializer.HasCircularReferences(entry.Value))
                    {
                        if (entry.Value is IEnumerable enumerable)
                        {
                            to[entry.Key] = entry.Value;
                        }
                        else
                        {
                            to[entry.Key] = entry.Value.ToSafePartialObjectDictionary();
                        }
                    }
                    else
                    {
                        to[entry.Key] = entry.Value.ToString();
                    }
                }
            }
            return to;
        }
    }
}
