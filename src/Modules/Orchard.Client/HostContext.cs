using Orchard.Client.Web;
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
    public static class HostContext
    {
        /// <summary>
        /// 
        /// </summary>
        public static ServiceStackHost AppHost => ServiceStackHost.Instance;
        public static IContentTypes ContentTypes => AssertAppHost().ContentTypes;
        internal static ServiceStackHost AssertAppHost()
        {
            if (ServiceStackHost.Instance == null)
                throw new Exception(
                    "ServiceStack: AppHost does not exist or has not been initialized. " +
                    "Make sure you have created an AppHost and started it with 'new AppHost().Init();' " +
                    " in your Global.asax Application_Start() or alternative Application StartUp");

            return ServiceStackHost.Instance;
        }
        public static T TryResolve<T>() => AssertAppHost().TryResolve<T>();
    }
}
