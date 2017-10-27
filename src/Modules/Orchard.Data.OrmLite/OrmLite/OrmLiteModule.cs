using Autofac;
using Orchard.Data.OrmLite.Repositories;
using Orchard.Domain.Entities;
using Orchard.Domain.Repositories;
using ServiceStack;
using ServiceStack.OrmLite;
using Module = Autofac.Module;

namespace Orchard.Data.OrmLite
{
    public class OrmLiteModule : Module
    {
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
            builder.RegisterGeneric(typeof(OrmLiteRepositoryBase<>)).As(typeof(IRepository<>)).InstancePerDependency();
            builder.RegisterGeneric(typeof(OrmLiteRepositoryBase<,>)).As(typeof(IRepository<,>)).InstancePerDependency();
        }
        protected override void AttachToComponentRegistration(Autofac.Core.IComponentRegistry componentRegistry, Autofac.Core.IComponentRegistration registration)
        {

        }
    }
}
