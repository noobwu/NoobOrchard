using Autofac;
using Orchard.Data.EntityFramework.Repositories;
using Module = Autofac.Module;

namespace Orchard.Data.EntityFramework
{
    public class EfRepositoryBaseModule : Module
    {
        /// <summary>
        /// .EntityFramework session factory object.
        /// </summary>
        //private ISessionFactory _sessionFactory;
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(EfRepositoryBase<>)).As(typeof(EfRepositoryBase<>)).InstancePerDependency();
            builder.RegisterGeneric(typeof(EfRepositoryBase<,>)).As(typeof(EfRepositoryBase<,>)).InstancePerDependency();
        }
        protected override void AttachToComponentRegistration(Autofac.Core.IComponentRegistry componentRegistry, Autofac.Core.IComponentRegistration registration)
        {

        }

    }
}
