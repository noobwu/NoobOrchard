using Autofac;
using Orchard.Data.OrmLite.Repositories;
using Orchard.Domain.Entities;
using Orchard.Domain.Repositories;
using ServiceStack;
using ServiceStack.OrmLite;
using Module = Autofac.Module;

namespace Orchard.Data.OrmLite
{
    public class OrmLiteRepositoryModule : Module
    {
        private bool registerGeneric;
        public OrmLiteRepositoryModule(bool registerGeneric=true)
        {
            this.registerGeneric = registerGeneric;
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void Load(ContainerBuilder builder)
        {
            //查询的时候过滤掉有删除标识的记录
            OrmLiteConfig.SqlExpressionSelectFilter = q =>
            {
                if (q.ModelDef.ModelType.HasInterface(typeof(ISoftDelete)))
                {
                    q.Where<ISoftDelete>(x => x.DeleteFlag != true);
                }
            };
            if (registerGeneric)
            {
                //builder.RegisterGeneric(typeof(OrmLiteRepositoryBase<>)).As(typeof(OrmLiteRepositoryBase<>)).InstancePerDependency();
                //builder.RegisterGeneric(typeof(OrmLiteRepositoryBase<,>)).As(typeof(OrmLiteRepositoryBase<,>)).InstancePerDependency();

                builder.RegisterGeneric(typeof(OrmLiteRepositoryBase<>)).As(typeof(IRepository<>)).InstancePerDependency();
                builder.RegisterGeneric(typeof(OrmLiteRepositoryBase<,>)).As(typeof(IRepository<,>)).InstancePerDependency();
            }
        }
        protected override void AttachToComponentRegistration(Autofac.Core.IComponentRegistry componentRegistry, Autofac.Core.IComponentRegistration registration)
        {

        }
    }
}
