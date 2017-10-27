using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Orchard.WebApi.OAuth2
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务

            // Web API 路由
            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            config.Routes.MapHttpRoute(
                name: "DefaultIndexApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { controller = "Default", action = "Index", id = RouteParameter.Optional }
            );
        }
    }
}
