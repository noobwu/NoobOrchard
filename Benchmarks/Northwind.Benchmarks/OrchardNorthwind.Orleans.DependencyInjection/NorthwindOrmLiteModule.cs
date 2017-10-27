using Autofac;
using Orchard.Data.OrmLite;
using OrchardNorthwind.Data.OrmLite;
using ServiceStack;
using ServiceStack.DataAnnotations;
using System;

namespace OrchardNorthwind.Orleans.DependencyInjection
{
    /// <summary>
    /// 
    /// </summary>
    public class NorthwindOrmLiteModule : Module
    {
        private bool registerGeneric;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="registerGeneric"></param>
        public NorthwindOrmLiteModule(bool registerGeneric = true)
        {
            this.registerGeneric = registerGeneric;
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void Load(ContainerBuilder builder)
        {
            InitOrmLiteMapping();
            builder.RegisterModule(new OrmLiteRepositoryModule(registerGeneric));
        }
        protected override void AttachToComponentRegistration(Autofac.Core.IComponentRegistry componentRegistry, Autofac.Core.IComponentRegistration registration)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        private void InitOrmLiteMapping()
        {
            NorthwindMappings.InitAdminOrmLiteMapping();
        }


    }
}
