using Autofac;
using Orchard.Data.EntityFrameworkCore.Repositories;
using Module = Autofac.Module;

namespace Orchard.Data.EntityFrameworkCore
{
    public class EfCoreRepositoryBaseModule : Module
    {
        /// <summary>
        /// .EntityFramework session factory object.
        /// </summary>
        //private ISessionFactory _sessionFactory;
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(EfCoreRepositoryBase<>)).As(typeof(EfCoreRepositoryBase<>)).InstancePerDependency();
            builder.RegisterGeneric(typeof(EfCoreRepositoryBase<,>)).As(typeof(EfCoreRepositoryBase<,>)).InstancePerDependency();
        }
        protected override void AttachToComponentRegistration(Autofac.Core.IComponentRegistry componentRegistry, Autofac.Core.IComponentRegistration registration)
        {

        }

    }
}
