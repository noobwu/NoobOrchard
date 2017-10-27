using Autofac;
using Orchard.Data.OrmLite.Tests.Entities;
using Orchard.Tests.Common.Domain.Entities;
using ServiceStack;
using ServiceStack.DataAnnotations;
using ServiceStack.Logging;
using ServiceStack.OrmLite;
using System;
using System.Configuration;
using System.Data;
using System.IO;

namespace Orchard.Data.OrmLite.Tests
{
    public abstract class OrmLiteTestBase
    {
        /// <summary>
        /// Parallel开始索引（含）。
        /// </summary>
        protected const int ParallelFromExclusive = 0;
        /// <summary>
        /// Parallel结束索引（不含）。
        /// </summary>
        protected const int ParallelToExclusive = 1000;
        /// <summary>
        /// 
        /// </summary>
        protected const int PageIndex = 1;
        /// <summary>
        /// 
        /// </summary>
        protected const int PageSize = 20;
        protected Autofac.IContainer Container;
        protected OrmLiteConnectionFactory DbFactory;
        protected IDbConnection Connection;
        protected virtual void Cleanup()
        {
            Container?.Dispose();
        }
        protected virtual void Init()
        {
            string connectionString =ConfigurationManager.ConnectionStrings["Test"].ConnectionString;
            DbFactory = new OrmLiteConnectionFactory(connectionString, SqlServer2014Dialect.Provider);
            //LogManager.LogFactory = new ConsoleLogFactory(debugEnabled: true);
            //LogManager.LogFactory = new ServiceStack.Logging.NLogger.NLogFactory();
            string logConfigFile = "log4net.config";
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, logConfigFile);
            LogManager.LogFactory = new ServiceStack.Logging.Log4Net.Log4NetFactory(filePath);

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
            InitOrmLiteMappings();

            var builder = new ContainerBuilder();
            builder.Register(c => DbFactory).InstancePerDependency();
            builder.RegisterModule(new OrmLiteModule());
            Register(builder);
            Container = builder.Build();
        }
        protected abstract void Register(ContainerBuilder builder);

        private void InitOrmLiteMappings()
        {
            var admAreaTestType = typeof(AdmAreaTest);
            admAreaTestType.AddAttributes(new Attribute[] { new AliasAttribute("wt_adm_area_test") });
            admAreaTestType.GetProperty("Id").AddAttributes(new Attribute[] { new PrimaryKeyAttribute(), new AutoIncrementAttribute(), new AliasAttribute("ID") });

            //typeof(AdmArea).AddAttributes(new AliasAttribute("wt_adm_area"));//设置对应的表名
            //var admAreaType = typeof(AdmArea);
            //Task.Factory.StartNew(() =>
            //{
            //    admAreaType.AddAttributes(new Attribute[] { new AliasAttribute("wt_adm_area") });
            //    admAreaType.GetProperty("Id").AddAttributes(new Attribute[] { new AutoIncrementAttribute() });
            //    //admAreaType.GetProperty("AreaName").AddAttributes(new Attribute[] { new AliasAttribute("AreaName1") });

            //});
            //var admAreaDefinition = ModelDefinition<AdmArea>.Definition;
            //var admAreaMapDefinition = ModelDefinition<AdmAreaMap>.Definition;
            //admAreaDefinition.Alias = admAreaMapDefinition.Alias;
            ////admAreaDefinition.Name = admAreaMapDefinition.Name;
            var admAreaType = typeof(AdmArea);
            admAreaType.AddAttributes(new Attribute[] { new AliasAttribute("wt_adm_area") });
            admAreaType.GetProperty("Id").AddAttributes(new Attribute[] { new PrimaryKeyAttribute(), new AutoIncrementAttribute() });

            var admUserRightsType = typeof(AdmUserRights);
            admUserRightsType.AddAttributes(new Attribute[] { new AliasAttribute("wt_adm_user_rights") });
            admUserRightsType.GetProperty("UserRightID").AddAttributes(new Attribute[] { new PrimaryKeyAttribute(), new AutoIncrementAttribute() });
            admUserRightsType.GetProperty("Id").AddAttributes(new Attribute[] { new ServiceStack.DataAnnotations.AliasAttribute("UserRightID") });

            var admUserRightsExt = typeof(AdmUserRightsExt);
            admUserRightsExt.GetProperty("IDPath").AddAttributes(new Attribute[] { new BelongToAttribute(typeof(AdmRightsType)) });


            var admRightsType = typeof(AdmRights);
            admRightsType.AddAttributes(new Attribute[] { new AliasAttribute("wt_adm_rights") });
            admRightsType.GetProperty("RightsID").AddAttributes(new Attribute[] { new PrimaryKeyAttribute(), new AutoIncrementAttribute() });


            var admRightsTypeType = typeof(AdmRightsType);
            admRightsTypeType.AddAttributes(new Attribute[] { new AliasAttribute("wt_adm_rights_type") });
            admRightsTypeType.GetProperty("RightsTypeID").AddAttributes(new Attribute[] { new PrimaryKeyAttribute(), new AutoIncrementAttribute() });

        }
    }
}
