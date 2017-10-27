using NLog;
using Orchard.OAuth2.OwinOAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Orchard.OAuth2.Mvc
{
    public class MvcApplication : ApplicationBase
    {
        private readonly ILogger logger;
        public MvcApplication()
        {
            logger = LogManager.GetCurrentClassLogger();
        }
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Error(object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;
            var result = HandleException(logger);
            if (result)
            {
                context.Response.ContentType = "application/json";
                context.Response.Write("{\"code\":\"0\",  \"msg\":\"处理你的请求时出错。\"}");
                return;
            }

        }
    }
}
