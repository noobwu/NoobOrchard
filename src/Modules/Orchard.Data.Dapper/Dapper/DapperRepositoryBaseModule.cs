using Autofac;
using Orchard.Data.Dapper.Repositories;
using Orchard.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Data.Dapper
{
    public class DapperRepositoryBaseModule : Module
    {
        /// <summary>
        /// NHibernate session factory object.
        /// </summary>
        //private ISessionFactory _sessionFactory;
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(DapperRepositoryBase<>)).As(typeof(DapperRepositoryBase<>)).InstancePerDependency();
            builder.RegisterGeneric(typeof(DapperRepositoryBase<,>)).As(typeof(DapperRepositoryBase<,>)).InstancePerDependency();
        }
        protected override void AttachToComponentRegistration(Autofac.Core.IComponentRegistry componentRegistry, Autofac.Core.IComponentRegistration registration)
        {

        }
    }
}
