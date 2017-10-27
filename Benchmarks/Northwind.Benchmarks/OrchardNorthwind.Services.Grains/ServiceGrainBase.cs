using NoobOrleans.Core;
using Orchard.Domain.Entities;
using Orchard.Logging;
using Orchard;
using Orleans;
namespace OrchardNorthwind.Services.Grains
{

    /// <summary>
    /// Grain相关操作
    /// </summary>
    public abstract class ServiceGrainBase<TEntity, TPrimaryKey> : Grain
     where TEntity : class, IEntity<TPrimaryKey>
    {
        #region Fields
        protected readonly ILogger Logger;
        protected readonly IGrainFactory InjectedGrainFactory;
        protected readonly ContainerContext Container;
        #endregion Fields

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="containerContext">containerContext</param>
        /// <param name="grainFactory">grainFactory</param>
        public ServiceGrainBase(ContainerContext containerContext, IGrainFactory grainFactory)
        {
            //Autofac DependencyInjection
            //var collection = new ServiceCollection();
            //var factory = (IServiceProviderFactory<ContainerBuilder>)ServiceProvider.GetService(typeof(IServiceProviderFactory<ContainerBuilder>));//暂时不支持 为空
            //if (factory != null)
            //{
            //    var builder = factory.CreateBuilder(collection);
            //}
            Container = containerContext;
            InjectedGrainFactory = grainFactory;
            ILoggerFactory loggerFactory = containerContext.Resolve<ILoggerFactory>();
            if (loggerFactory != null)
            {
                Logger = loggerFactory.GetLogger(this.GetType());
            }
        }

        #endregion Ctor

        /// <summary>
        /// 
        /// </summary>
        public virtual void Dispose()
        {

        }
    }
}
