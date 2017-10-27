using Autofac;
using Autofac.Integration.Web;
using Orchard.Tests;
using System;
using System.Collections.Generic;
using System.Web;

namespace Orchard.Web.Mvc.Tests
{
    public class DefaultDisplayManagerTests : ContainerTestBase
    {
        public class TestWorkContextAccessor : IWorkContextAccessor
        {
            private readonly WorkContext _workContext;
            public TestWorkContextAccessor(WorkContext workContext)
            {
                _workContext = workContext;
            }

            public WorkContext GetContext(HttpContextBase httpContext)
            {
                return _workContext;
            }

            public IWorkContextScope CreateWorkContextScope(HttpContextBase httpContext)
            {
                throw new NotImplementedException();
            }

            public WorkContext GetContext()
            {
                return _workContext;
            }

            public IWorkContextScope CreateWorkContextScope()
            {
                throw new NotImplementedException();
            }
        }
        public class TestWorkContext : WorkContext
        {
            readonly IDictionary<string, object> _state = new Dictionary<string, object>();
            public IContainerProvider ContainerProvider { get; set; }

            public override T Resolve<T>()
            {
                if (typeof(T) == typeof(ILifetimeScope))
                {
                    return (T)ContainerProvider.RequestLifetime;
                }

                throw new NotImplementedException();
            }

            public override object Resolve(Type serviceType)
            {
                throw new NotImplementedException();
            }

            public override bool TryResolve<T>(out T service)
            {
                throw new NotImplementedException();
            }

            public override bool TryResolve(Type serviceType, out object service)
            {
                throw new NotImplementedException();
            }

            public override T GetState<T>(string name)
            {
                object value;
                return _state.TryGetValue(name, out value) ? (T)value : default(T);
            }

            public override void SetState<T>(string name, T value)
            {
                _state[name] = value;
            }
        }

    }
}
