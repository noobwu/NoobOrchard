using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using Autofac.Core.Lifetime;
using Autofac.Integration.Mvc;
using Orchard.Infrastructure;
using System.Runtime.CompilerServices;
using Autofac.Core;

namespace Orchard.Web.Mvc.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    public class ContainerContext
    {
        #region Properties

        /// <summary>
        /// Gets the singleton Nop engine used to access Nop services.
        /// </summary>
        public static ContainerContext Current
        {
            get
            {
                if (Singleton<ContainerContext>.Instance == null)
                {
                    Initialize();
                }
                return Singleton<ContainerContext>.Instance;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual IContainer Container { get; set; }
        #endregion

        #region Methods start

        /// <summary>
        /// Initializes a static instance 
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static ContainerContext Initialize()
        {
            if (Singleton<ContainerContext>.Instance == null)
            {
                Singleton<ContainerContext>.Instance = new ContainerContext();
            }
            return Singleton<ContainerContext>.Instance;
        }

        /// <summary>
        /// Sets the static context instance to the supplied context. Use this method to supply your own context implementation.
        /// </summary>
        /// <param name="context">The context to use.</param>
        /// <remarks>Only use this method if you know what you're doing.</remarks>
        public static void Replace(ContainerContext context)
        {
            Singleton<ContainerContext>.Instance = context;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual T Resolve<T>(params Parameter[] parameters) where T : class
        {
            return Resolve<T>(string.Empty,null,parameters);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="scope"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual T Resolve<T>(string key = "",ILifetimeScope scope = null, params Parameter[] parameters) where T : class
        {
            if (scope == null)
            {
                //no scope specified
                scope = Scope();
            }
            if (string.IsNullOrEmpty(key))
            {
                return scope.Resolve<T>(parameters);
            }
            return scope.ResolveKeyed<T>(key,parameters);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public virtual T Resolve<T>(string key = "", ILifetimeScope scope = null) where T : class
        {
            if (scope == null)
            {
                //no scope specified
                scope = Scope();
            }
            if (string.IsNullOrEmpty(key))
            {
                return scope.Resolve<T>();
            }
            return scope.ResolveKeyed<T>(key);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public virtual object Resolve(Type type, ILifetimeScope scope = null)
        {
            if (scope == null)
            {
                //no scope specified
                scope = Scope();
            }
            return scope.Resolve(type);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public virtual T[] ResolveAll<T>(string key = "", ILifetimeScope scope = null)
        {
            if (scope == null)
            {
                //no scope specified
                scope = Scope();
            }
            if (string.IsNullOrEmpty(key))
            {
                return scope.Resolve<IEnumerable<T>>().ToArray();
            }
            return scope.ResolveKeyed<IEnumerable<T>>(key).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scope"></param>
        /// <returns></returns>
        public virtual T ResolveUnregistered<T>(ILifetimeScope scope = null) where T : class
        {
            return ResolveUnregistered(typeof(T), scope) as T;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public virtual object ResolveUnregistered(Type type, ILifetimeScope scope = null)
        {
            //if (scope == null)
            //{
            //    //no scope specified
            //    scope = Scope();
            //}
            var constructors = type.GetConstructors();
            foreach (var constructor in constructors)
            {
                try
                {
                    var parameters = constructor.GetParameters();
                    var parameterInstances = new List<object>();
                    foreach (var parameter in parameters)
                    {
                       //var service =Container.Resolve(parameter.ParameterType);
                        var service = Resolve(parameter.ParameterType, scope);
                        if (service == null) throw new Exception("Unkown dependency");
                        parameterInstances.Add(service);
                    }
                    return Activator.CreateInstance(type, parameterInstances.ToArray());
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            throw new Exception("No contructor was found that had all the dependencies satisfied.");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="scope"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public virtual bool TryResolve(Type serviceType, ILifetimeScope scope, out object instance)
        {
            if (scope == null)
            {
                //no scope specified
                scope = Scope();
            }
            return scope.TryResolve(serviceType, out instance);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public virtual bool IsRegistered(Type serviceType, ILifetimeScope scope = null)
        {
            if (scope == null)
            {
                //no scope specified
                scope = Scope();
            }
            return scope.IsRegistered(serviceType);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public virtual object ResolveOptional(Type serviceType, ILifetimeScope scope = null)
        {
            if (scope == null)
            {
                //no scope specified
                scope = Scope();
            }
            return scope.ResolveOptional(serviceType);
        }
        /// <summary>
        ///   been added.
        /// </summary>
        /// <returns></returns>
        public virtual ILifetimeScope Scope()
        {
            try
            {
                if (HttpContext.Current != null)
                    return AutofacDependencyResolver.Current.RequestLifetimeScope;

                //when such lifetime scope is returned, you should be sure that it'll be disposed once used (e.g. in schedule tasks)
                return Container.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag);
            }
            catch (Exception)
            {
                //we can get an exception here if RequestLifetimeScope is already disposed
                //for example, requested in or after "Application_EndRequest" handler
                //but note that usually it should never happen

                //when such lifetime scope is returned, you should be sure that it'll be disposed once used (e.g. in schedule tasks)
                return Container.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag);
            }
        }
        #endregion end
    }
}
