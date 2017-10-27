using Autofac;
using System.Web;

namespace Orchard
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class WorkContextExtensions
    {
        public static WorkContext GetLogicalContext(this IWorkContextAccessor workContextAccessor)
        {
            var wca = workContextAccessor as ILogicalWorkContextAccessor;
            return wca != null ? wca.GetLogicalContext() : null;
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
