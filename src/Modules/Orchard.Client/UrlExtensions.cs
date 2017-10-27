using Orchard.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Client
{
    /// <summary>
    /// 
    /// </summary>
     public static class UrlExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetOperationName(this Type type)
        {
            //Need to expand Arrays of Generic Types like Nullable<Byte>[]
            if (type.IsArray)
            {
                return type.GetElementType().ExpandTypeName() + "[]";
            }

            string fullname = type.FullName;
            int genericPrefixIndex = type.IsGenericParameter ? 1 : 0;

            if (fullname == null)
                return genericPrefixIndex > 0 ? "'" + type.Name : type.Name;

            int startIndex = type.Namespace != null ? type.Namespace.Length + 1 : 0; //trim namespace + "."
            int endIndex = fullname.IndexOf("[[", startIndex);  //Generic Fullname
            if (endIndex == -1)
                endIndex = fullname.Length;

            char[] op = new char[endIndex - startIndex + genericPrefixIndex];
            char cur;

            for (int i = startIndex; i < endIndex; i++)
            {
                cur = fullname[i];
                op[i - startIndex + genericPrefixIndex] = cur != '+' ? cur : '.';
            }

            if (genericPrefixIndex > 0)
                op[0] = '\'';

            return new string(op);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ExpandTypeName(this Type type)
        {
            if (type.IsGenericType)
                return ExpandGenericTypeName(type);

            return type.GetOperationName();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ExpandGenericTypeName(Type type)
        {
            var nameOnly = type.Name.LeftPart('`');

            var sb = StringBuilderCache.Allocate();
            foreach (var arg in type.GetGenericArguments())
            {
                if (sb.Length > 0)
                    sb.Append(",");

                sb.Append(arg.ExpandTypeName());
            }

            var fullName = $"{nameOnly}<{StringBuilderCache.ReturnAndFree(sb)}>";
            return fullName;
        }
    }
}
