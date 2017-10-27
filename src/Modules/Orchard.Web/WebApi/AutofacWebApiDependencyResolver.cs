using Autofac;
using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;

namespace Orchard.Web.WebApi
{
    public class AutofacWebApiDependencyResolver : IDependencyResolver
    {
        readonly ILifetimeScope _container;
        readonly IDependencyScope _rootDependencyScope;

        //internal static readonly string ApiRequestTag = "AutofacWebRequest";

        public AutofacWebApiDependencyResolver(ILifetimeScope container)
        {
            if (container == null) throw new ArgumentNullException("container");

            _container = container;
            _rootDependencyScope = new AutofacWebApiDependencyScope(container);
        }
        /// <summary>
        /// 
        /// </summary>
        public ILifetimeScope Container
        {
            get { return _container; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public object GetService(Type serviceType)
        {
            return _rootDependencyScope.GetService(serviceType);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _rootDependencyScope.GetServices(serviceType);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDependencyScope BeginScope()
        {
            ILifetimeScope lifetimeScope = _container.BeginLifetimeScope();
            return new AutofacWebApiDependencyScope(lifetimeScope);
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            _rootDependencyScope.Dispose();
        }
    }
}
