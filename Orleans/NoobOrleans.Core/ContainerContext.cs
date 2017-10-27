using Autofac;
using Autofac.Core;
using Autofac.Core.Lifetime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NoobOrleans.Core
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ContainerContext
    {
        #region Properties
        /// <summary>
        /// 
        /// </summary>
        public virtual IContainer Container { get; set; }
        #endregion

        #region Methods start

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="scope"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual T Resolve<T>(string key,ILifetimeScope scope,params Parameter[] parameters)
        {
            if (scope == null)
            {
                //no scope specified
                return Resolve<T>(key,parameters);
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
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual T Resolve<T>(string key,params Parameter[] parameters)
        {
            if (string.IsNullOrEmpty(key))
            {
                return Container.Resolve<T>(parameters);
            }
            return Container.ResolveKeyed<T>(key,parameters);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public virtual T Resolve<T>(string key, ILifetimeScope scope)
        {
            if (scope == null)
            {
                //no scope specified
                return Resolve<T>(key);
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
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public virtual T Resolve<T>(string key = "")
        {
            if (string.IsNullOrEmpty(key))
            {
                return Container.Resolve<T>();
            }
            return Container.ResolveKeyed<T>(key);
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
        /// <param name="type"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public virtual object Resolve(Type type)
        {
            return Container.Resolve(type);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public virtual T[] ResolveAll<T>(string key, ILifetimeScope scope)
        {
            if (scope == null)
            {
                //no scope specified
              return ResolveAll<T>(key);
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
        /// <param name="key"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public virtual T[] ResolveAll<T>(string key = "")
        {
            if (string.IsNullOrEmpty(key))
            {
                return Container.Resolve<IEnumerable<T>>().ToArray();
            }
            return Container.ResolveKeyed<IEnumerable<T>>(key).ToArray();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scope"></param>
        /// <returns></returns>
        public virtual T ResolveUnregistered<T>(ILifetimeScope scope) where T : class
        {
            return ResolveUnregistered(typeof(T), scope) as T;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public virtual object ResolveUnregistered(Type type, ILifetimeScope scope)
        {
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
               return  Container.TryResolve(serviceType,out instance);
            }
            return scope.TryResolve(serviceType, out instance);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public virtual bool IsRegistered(Type serviceType, ILifetimeScope scope)
        {
            if (scope == null)
            {
                //no scope specified
                return  Container.IsRegistered(serviceType);
            }
            return scope.IsRegistered(serviceType);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public virtual object ResolveOptional(Type serviceType, ILifetimeScope scope)
        {
            if (scope == null)
            {
                //no scope specified
                return Container.ResolveOptional(serviceType);
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

                //return Container.BeginLifetimeScope(a => { });
                //return ILifetimeScope scope= Container.BeginLifetimeScope();
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
