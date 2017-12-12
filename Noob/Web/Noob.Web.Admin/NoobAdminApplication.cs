using Autofac;
using Orchard.Environment;
using Orchard.Logging;
using Orchard.Logging.NLogger;
using Orchard.Web.Mvc.Infrastructure;
using Orchard.Web.Web;
using System;
using System.Web;
using System.Web.Mvc;

namespace Noob.Web.Admin
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class NoobAdminApplication : ApplicationBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        public abstract void Register(ContainerBuilder builder);
        /// <summary>
        /// 
        /// </summary>
        public abstract void InitApplicationStart();

    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual  void Application_Error(object sender, EventArgs e)
        {
            ILogger logger = null;
            var logFactory = ContainerContext.Current.Resolve<ILoggerFactory>();
            if (logFactory != null)
            {
                logger = logFactory.GetLogger(GetType());
            }
            HttpContext context = HttpContext.Current;
            var result= HandleException(logger);
            if (result)
            {
                context.Response.ContentType = "application/json";
                context.Response.Write("{\"code\":\"0\",  \"msg\":\"处理你的请求时出错。\"}");
                return;
            }

        }

    }
}