using Autofac;
using Noob.Data.OrmLite;
using Noob.IServices;
using Noob.Services.OrmLite;
using Orchard.Utility;
using ServiceStack.OrmLite;

namespace Noob.Web.Admin.EasyUI.OrmLite
{
    /// <summary>
    /// 
    /// </summary>
    public class NoobAdminOrmLiteDataModule : Autofac.Module
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

            RegisterOrmLite(builder);

            RegisterOrmLiteServices(builder);


        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        private void RegisterOrmLite(ContainerBuilder builder)
        {


            //LogManager.LogFactory = new ConsoleLogFactory(debugEnabled: true);
            //LogManager.LogFactory = new ServiceStack.Logging.NLogger.NLogFactory();
            string logConfigFile = "/App_Data/Configs/log4net.config";
            var filePath = Utils.GetMapPath(logConfigFile);
            //LogManager.LogFactory = new ServiceStack.Logging.Log4Net.Log4NetFactory(filePath);

            //logConfigFile = "entlib5.config";
            //LogManager.LogFactory = new ServiceStack.Logging.EntLib5.EntLib5Factory(logConfigFile);

            // LogManager.LogFactory = new ServiceStack.Logging.EventLog.EventLogFactory("Orchard.Data.OrmLite.Tests", "Application",true);
            // LogManager.LogFactory = new ServiceStack.Logging.Serilog.SerilogFactory();
            //LogManager.LogFactory = new ServiceStack.Logging.Slack.SlackLogFactory("{GeneratedSlackUrlFromCreatingIncomingWebhook}", debugEnabled: true)
            //{
            //    //Alternate default channel than one specified when creating Incoming Webhook.
            //    DefaultChannel = "other-default-channel",
            //    //Custom channel for Fatal logs. Warn, Info etc will fallback to DefaultChannel or 
            //    //channel specified when Incoming Webhook was created.
            //    FatalChannel = "more-grog-logs",
            //    //Custom bot username other than default
            //    BotUsername = "Guybrush Threepwood",
            //    //Custom channel prefix can be provided to help filter logs from different users or environments. 
            //    ChannelPrefix = System.Security.Principal.WindowsIdentity.GetCurrent().Name
            //};

            var connectionString = @"Data Source=.;Initial Catalog=Test;User ID=sa;Password=123456";
            var dbFactory = new OrmLiteConnectionFactory(connectionString, SqlServer2014Dialect.Provider);
            builder.Register(c => dbFactory).InstancePerDependency();
            builder.RegisterModule(new NoobOrmLiteModule());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        private void RegisterOrmLiteServices(ContainerBuilder builder)
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