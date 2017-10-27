using System.Reflection;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Orchard.Web.Mvc.Helpers
{
    internal static class MethodInfoHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static bool IsJsonResult(MethodInfo method)
        {
            return typeof(JsonResult).IsAssignableFrom(method.ReturnType) ||
                   typeof(Task<JsonResult>).IsAssignableFrom(method.ReturnType);
        }
    }
}
