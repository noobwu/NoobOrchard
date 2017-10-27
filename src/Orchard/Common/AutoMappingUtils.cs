using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Orchard
{
    public delegate object GetMemberDelegate(object instance);
    public delegate object GetMemberDelegate<T>(T instance);
    public delegate void SetMemberDelegate(object instance, object value);
    public delegate void SetMemberDelegate<T>(T instance, object value);
    public delegate void SetMemberRefDelegate(ref object instance, object propertyValue);
    public delegate void SetMemberRefDelegate<T>(ref T instance, object value);
    /// <summary>
    /// 
    /// </summary>
    public static class AutoMappingUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object GetProperty(this PropertyInfo propertyInfo, object obj)
        {
            if (propertyInfo == null || !propertyInfo.CanRead)
                return null;

            var getMethod = propertyInfo.GetMethodInfo();
            return getMethod?.Invoke(obj, TypeConstants.EmptyObjectArray);
        }
    }

}
