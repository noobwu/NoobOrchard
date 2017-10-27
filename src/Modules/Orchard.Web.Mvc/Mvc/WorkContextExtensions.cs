using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using System.Web.Http.Controllers;

namespace Orchard.Web.Mvc
{
    /// <summary>
    /// 
    /// </summary>
    public static class WorkContextExtensions {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="workContextAccessor"></param>
        /// <param name="controllerContext"></param>
        /// <returns></returns>
        public static WorkContext GetContext(this IWorkContextAccessor workContextAccessor, ControllerContext controllerContext) {
            return workContextAccessor.GetContext(controllerContext.RequestContext.HttpContext);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workContextAccessor"></param>
        /// <returns></returns>
        public static WorkContext GetLogicalContext(this IWorkContextAccessor workContextAccessor) {
            var wca = workContextAccessor as ILogicalWorkContextAccessor;
            return wca != null ? wca.GetLogicalContext() : null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestContext"></param>
        /// <returns></returns>
        public static WorkContext GetWorkContext(this RequestContext requestContext) {
            if (requestContext == null)
                return null;

            var routeData = requestContext.RouteData;
            if (routeData == null || routeData.DataTokens == null)
                return null;
            
            object workContextValue;
            if (!routeData.DataTokens.TryGetValue("IWorkContextAccessor", out workContextValue)) {
                workContextValue = FindWorkContextInParent(routeData);
            }

            if (!(workContextValue is IWorkContextAccessor))
                return null;

            var workContextAccessor = (IWorkContextAccessor)workContextValue;
            return workContextAccessor.GetContext(requestContext.HttpContext);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <returns></returns>
        public static WorkContext GetWorkContext(this HttpControllerContext controllerContext) {
            if (controllerContext == null)
                return null;

            var routeData = controllerContext.RouteData;
            if (routeData == null || routeData.Values == null)
                return null;

            object workContextValue;
            if (!routeData.Values.TryGetValue("IWorkContextAccessor", out workContextValue)) {
                return null;
            }

            if (workContextValue == null || !(workContextValue is IWorkContextAccessor))
                return null;

            var workContextAccessor = (IWorkContextAccessor)workContextValue;
            return workContextAccessor.GetContext();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="routeData"></param>
        /// <returns></returns>
        private static object FindWorkContextInParent(RouteData routeData) {
            object parentViewContextValue;
            if (!routeData.DataTokens.TryGetValue("ParentActionViewContext", out parentViewContextValue)
                || !(parentViewContextValue is ViewContext)) {
                return null;
            }

            var parentRouteData = ((ViewContext)parentViewContextValue).RouteData;
            if (parentRouteData == null || parentRouteData.DataTokens == null)
                return null;

            object workContextValue;
            if (!parentRouteData.DataTokens.TryGetValue("IWorkContextAccessor", out workContextValue)) {
                workContextValue = FindWorkContextInParent(parentRouteData);
            }

            return workContextValue;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <returns></returns>
        public static WorkContext GetWorkContext(this ControllerContext controllerContext) {
            if (controllerContext == null)
                return null;

            return GetWorkContext(controllerContext.RequestContext);
        }

        public static IWorkContextScope CreateWorkContextScope(this ILifetimeScope lifetimeScope, HttpContextBase httpContext) {
            return lifetimeScope.Resolve<IWorkContextAccessor>().CreateWorkContextScope(httpContext);
        }

        public static IWorkContextScope CreateWorkContextScope(this ILifetimeScope lifetimeScope) {
            return lifetimeScope.Resolve<IWorkContextAccessor>().CreateWorkContextScope();
        }
    }
}
