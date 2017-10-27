using Autofac;
using System.Web;
using System.Web.Http.Controllers;

namespace Orchard
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class WorkContextExtensions
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <returns></returns>
        public static WorkContext GetWorkContext(this HttpControllerContext controllerContext)
        {
            if (controllerContext == null)
                return null;

            var routeData = controllerContext.RouteData;
            if (routeData == null || routeData.Values == null)
                return null;

            object workContextValue;
            if (!routeData.Values.TryGetValue("IWorkContextAccessor", out workContextValue))
            {
                return null;
            }

            if (workContextValue == null || !(workContextValue is IWorkContextAccessor))
                return null;

            var workContextAccessor = (IWorkContextAccessor)workContextValue;
            return workContextAccessor.GetContext();
        }

        public static IWorkContextScope CreateWorkContextScope(this ILifetimeScope lifetimeScope, HttpContextBase httpContext)
        {
            return lifetimeScope.Resolve<IWorkContextAccessor>().CreateWorkContextScope(httpContext);
        }

        public static IWorkContextScope CreateWorkContextScope(this ILifetimeScope lifetimeScope)
        {
            return lifetimeScope.Resolve<IWorkContextAccessor>().CreateWorkContextScope();
        }
    }
}
