using Autofac;
using Orchard.Client.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Client
{
    /// <summary>
    /// 
    /// </summary>
    public class ServiceStackHost
    {
        /// <summary>
        /// When the AppHost was instantiated.
        /// </summary>
        public DateTime StartedAt { get; set; }
        /// <summary>
        /// When the Init function was done.
        /// Called at begin of <see cref="OnAfterInit"/>
        /// </summary>
        public DateTime? AfterInitAt { get; set; }
        /// <summary>
        /// When all configuration was completed.
        /// Called at the end of <see cref="OnAfterInit"/>
        /// </summary>
        public DateTime? ReadyAt { get; set; }
        /// <summary>
        /// The assemblies reflected to find api services.
        /// These can be provided in the constructor call.
        /// </summary>
        public List<Assembly> ServiceAssemblies { get; private set; }

        /// <summary>
        /// Wether AppHost configuration is done.
        /// Note: It doesn't mean the start function was called.
        /// </summary>
        public bool HasStarted => ReadyAt != null;

        /// <summary>
        /// Wether AppHost is ready configured and either ready to run or already running.
        /// Equals <see cref="HasStarted"/>
        /// </summary>
        public static bool IsReady() => Instance?.ReadyAt != null;
        /// <summary>
        /// 
        /// </summary>
        public string ServiceName { get; set; }
 
        public static ServiceStackHost Instance { get; protected set; }

        public IContentTypes ContentTypes { get; set; }
        /// <summary>
        /// The AppHost.Container. Note: it is not thread safe to register dependencies after AppStart.
        /// </summary>
        public virtual IContainer Container { get; private set; }

        public ServiceStackHost(string serviceName, params Assembly[] assembliesWithServices)
        {
            ServiceName = serviceName;
            ServiceAssemblies = assembliesWithServices.ToList();
        }

        /// <summary>
        /// Tries to resolve type through the ioc container of the AppHost. 
        /// Can return null.
        /// </summary>
        public virtual T TryResolve<T>()
        {
            bool isResolve=this.Container.TryResolve<T>(out T instance);
            return instance;
        }
    }
}
