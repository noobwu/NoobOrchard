using Autofac;
using Orchard.Data.NHibernate.Repositories;
using Module = Autofac.Module;

namespace Orchard.Data.NHibernate
{
    public class NHibernateModule : Module
    {
        /// <summary>
        /// NHibernate session factory object.
        /// </summary>
        //private ISessionFactory _sessionFactory;
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(NhRepositoryBase<>)).As(typeof(Domain.Repositories.IRepository<>)).InstancePerDependency();
            builder.RegisterGeneric(typeof(NhRepositoryBase<,>)).As(typeof(Domain.Repositories.IRepository<,>)).InstancePerDependency();
        }
        protected override void AttachToComponentRegistration(Autofac.Core.IComponentRegistry componentRegistry, Autofac.Core.IComponentRegistration registration)
        {

        }
    }
}
