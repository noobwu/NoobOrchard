using Autofac;
using Orchard.Data.EntityFrameworkCore.Repositories;
using Module = Autofac.Module;

namespace Orchard.Data.EntityFrameworkCore
{
    public class EntityFrameworkCoreModule : Module
    {
        /// <summary>
        /// .EntityFramework session factory object.
        /// </summary>
        //private ISessionFactory _sessionFactory;
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(EfCoreRepositoryBase<>)).As(typeof(Domain.Repositories.IRepository<>)).InstancePerDependency();
            builder.RegisterGeneric(typeof(EfCoreRepositoryBase<,>)).As(typeof(Domain.Repositories.IRepository<,>)).InstancePerDependency();
        }
        protected override void AttachToComponentRegistration(Autofac.Core.IComponentRegistry componentRegistry, Autofac.Core.IComponentRegistration registration)
        {

        }

    }
}
