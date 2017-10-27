// Copyright (c) ServiceStack, Inc. All Rights Reserved.
// License: https://raw.github.com/ServiceStack/ServiceStack/master/license.txt

using System;
using System.Reflection;

namespace Orchard.Reflection.Extensions
{
    public static class AttributeExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetDescription(this Type type)
        {
            var componentDescAttr = type.FirstAttribute<System.ComponentModel.DescriptionAttribute>();
            if (componentDescAttr != null)
                return componentDescAttr.Description;
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mi"></param>
        /// <returns></returns>
        public static string GetDescription(this MemberInfo mi)
        {
            var componentDescAttr = mi.FirstAttribute<System.ComponentModel.DescriptionAttribute>();
            if (componentDescAttr != null)
                return componentDescAttr.Description;
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pi"></param>
        /// <returns></returns>
        public static string GetDescription(this ParameterInfo pi)
        {
            var componentDescAttr = pi.FirstAttribute<System.ComponentModel.DescriptionAttribute>();
            if (componentDescAttr != null)
                return componentDescAttr.Description;
            return null;
        }
    }
}
