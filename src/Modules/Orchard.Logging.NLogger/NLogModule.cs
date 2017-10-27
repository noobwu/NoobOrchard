using Autofac;
using Autofac.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Module = Autofac.Module;

namespace Orchard.Logging.NLogger
{
    /// <summary>
    /// 
    /// </summary>
    public class NLogModule : Module
    {
        private readonly ConcurrentDictionary<string, ILogger> _loggerCache;

        public NLogModule()
        {
            _loggerCache = new ConcurrentDictionary<string, ILogger>();
        }

        protected override void Load(ContainerBuilder moduleBuilder)
        {
            // by default, use Orchard's logger that delegates to Castle's logger factory
            //moduleBuilder.RegisterType<NLogFactory>().As<ILoggerFactory>().InstancePerLifetimeScope();
            var logFactory = new NLogFactory("~/App_Data/Configs/NLog.config");
            LoggerManager.LogFactory = logFactory;
            moduleBuilder.Register(c=>logFactory).As<ILoggerFactory>().SingleInstance();
            // call CreateLogger in response to the request for an ILogger implementation
            moduleBuilder.Register(GetLogger).As<ILogger>().InstancePerDependency();
        }

        protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
        {
            var implementationType = registration.Activator.LimitType;

            // build an array of actions on this type to assign loggers to member properties
            var injectors = BuildLoggerInjectors(implementationType).ToArray();

            // if there are no logger properties, there's no reason to hook the activated event
            if (!injectors.Any())
                return;

            // otherwise, whan an instance of this component is activated, inject the loggers on the instance
            registration.Activated += (s, e) =>
            {
                foreach (var injector in injectors)
                    injector(e.Context, e.Instance);
            };
        }

        private IEnumerable<Action<IComponentContext, object>> BuildLoggerInjectors(Type componentType)
        {
            // Look for settable properties of type "ILogger" 
            var loggerProperties = componentType
                .GetProperties(BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance)
                .Select(p => new
                {
                    PropertyInfo = p,
                    p.PropertyType,
                    IndexParameters = p.GetIndexParameters(),
                    Accessors = p.GetAccessors(false)
                })
                .Where(x => x.PropertyType == typeof(ILogger)) // must be a logger
                .Where(x => x.IndexParameters.Count() == 0) // must not be an indexer
                .Where(x => x.Accessors.Length != 1 || x.Accessors[0].ReturnType == typeof(void)); //must have get/set, or only set

            // Return an array of actions that resolve a logger and assign the property
            foreach (var entry in loggerProperties)
            {
                var propertyInfo = entry.PropertyInfo;

                yield return (ctx, instance) =>
                {
                    string component = componentType.ToString();
                    if (component != instance.GetType().ToString())
                    {
                        return;
                    }
                    var logger = _loggerCache.GetOrAdd(component, key => ctx.Resolve<ILogger>(new TypedParameter(typeof(Type), componentType)));
                    propertyInfo.SetValue(instance, logger, null);
                };
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private  ILogger GetLogger(IComponentContext context, IEnumerable<Parameter> parameters)
        {
            // return an ILogger in response to Resolve<ILogger>(componentTypeParameter)
            var loggerFactory = context.Resolve<ILoggerFactory>();
            var containingType = parameters.TypedAs<Type>();
            return loggerFactory.GetLogger(containingType);
        }
    }
}
