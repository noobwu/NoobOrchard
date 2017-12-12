using Autofac;
using Noob.Data.EntityFramework;
using Noob.IServices;
using Noob.Services.EntityFramework;
using Orchard.Data.EntityFramework;
using System.Data.Common;
using System.Data.SqlClient;

namespace Noob.Web.Admin.EasyUI.EntityFramework
{
    /// <summary>
    /// 
    /// </summary>
    public class NoobAdminEntityFrameworkDataModule : Autofac.Module
    {
        /// <summary>
        /// 
        /// </summary>
        protected override void Load(ContainerBuilder builder)
        {
            Init(builder);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        private void Init(ContainerBuilder builder)
        {
            RegisterEntityFramework(builder);
            RegisterEntityFrameworkServices(builder);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        private void RegisterEntityFramework(ContainerBuilder builder)
        {
            var _connectionString = @"Data Source=.;Database=Test; User=sa;Password=123456; Pooling=True; Max Pool Size=50;Connect Timeout=120;MultipleActiveResultSets=True;Max Pool Size=10; Min Pool Size=5;pooling = true;";
            DbConnection connection = new SqlConnection(_connectionString);
            // 注册 DbContext
            // NoobEfDbContext dbContext = new NoobEfDbContext(connection);
            //builder.RegisterInstance<EfDbContext>(dbContext).InstancePerDependency();
            // builder.RegisterInstance<EfDbContext>(dbContext);
            //builder.RegisterInstance<EfDbContext>(dbContext).SingleInstance();

            builder.Register(a => _connectionString).SingleInstance();
            //builder.RegisterType(typeof(NoobEfDbContext)).As(typeof(EfDbContext)).InstancePerDependency();
            builder.RegisterType(typeof(NoobEfDbContext)).As(typeof(EfDbContext)).InstancePerLifetimeScope();
            builder.RegisterModule(new NoobEntityFrameworkModule());

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        private void RegisterEntityFrameworkServices(ContainerBuilder builder)
        {
            //services
            /*这个作用域适用于嵌套的生命周期。一个使用Per Lifetime 作用域的component在一个 nested lifetime scope内最多有一个实例。
当对象特定于一个工作单元时，这个非常有用。比如，一个HTTP请求，每一个工作单元都会创建一个nested lifetime，如果在每一次HTTP请求中创建一个nested lifetime，那么其他使用 per-lifetime 的component在每次HTTP请求中只会拥有一个实例。
这种配置模型在其他容器中等价于per-HTTP-request, per-thread等。*/
            RegiserAdminService(builder);
            RegiserCmsService(builder);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        private void RegiserAdminService(ContainerBuilder builder)
        {
            builder.RegisterType<AdmRightsTypeService>().As<IAdmRightsTypeService>().InstancePerLifetimeScope();
            builder.RegisterType<AdmRightsService>().As<IAdmRightsService>().InstancePerLifetimeScope();
            builder.RegisterType<AdmGroupService>().As<IAdmGroupService>().InstancePerLifetimeScope();
            builder.RegisterType<AdmGroupRightsService>().As<IAdmGroupRightsService>().InstancePerLifetimeScope();
            builder.RegisterType<AdmUserGroupService>().As<IAdmUserGroupService>().InstancePerLifetimeScope();
            //使用Single Instance作用域，所有对父容器或者嵌套容器的请求都会返回同一个实例。
            builder.RegisterType<AdmUserService>().As<IAdmUserService>().SingleInstance();
            builder.RegisterType<AdmUserRightsService>().As<IAdmUserRightsService>().InstancePerLifetimeScope();
            builder.RegisterType<AdmMenuService>().As<IAdmMenuService>().InstancePerLifetimeScope();
            builder.RegisterType<AdmOrganizationService>().As<IAdmOrganizationService>().InstancePerLifetimeScope();
            builder.RegisterType<AdmConfigsService>().As<IAdmConfigsService>().InstancePerLifetimeScope();
            builder.RegisterType<AdmAreaService>().As<IAdmAreaService>().InstancePerLifetimeScope();
            builder.RegisterType<AdmPayConfigsService>().As<IAdmPayConfigsService>().InstancePerLifetimeScope();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        private void RegiserCmsService(ContainerBuilder builder)
        {
            builder.RegisterType<CmsAdvExternalService>().As<ICmsAdvExternalService>().InstancePerLifetimeScope();
            builder.RegisterType<CmsAdvPositionService>().As<ICmsAdvPositionService>().InstancePerLifetimeScope();
            builder.RegisterType<CmsAdvService>().As<ICmsAdvService>().InstancePerLifetimeScope();
            builder.RegisterType<CmsNewsCategoryService>().As<ICmsNewsCategoryService>().InstancePerLifetimeScope();
            builder.RegisterType<CmsNewsService>().As<ICmsNewsService>().InstancePerLifetimeScope();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="componentRegistry"></param>
        /// <param name="registration"></param>
        protected override void AttachToComponentRegistration(Autofac.Core.IComponentRegistry componentRegistry, Autofac.Core.IComponentRegistration registration)
        {

        }
    }
}