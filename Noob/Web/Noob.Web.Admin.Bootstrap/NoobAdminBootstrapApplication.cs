using Autofac;
using Autofac.Integration.Mvc;
using Metrics;
using NLog.Config;
using Orchard.Environment;
using Orchard.Environment.Configuration;
using Orchard.Logging.NLogger;
using Orchard.Performance;
using Orchard.Utility;
using Orchard.Web.Mvc.Infrastructure;
using System;
using System.IO;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Noob.Web.Admin.Bootstrap
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class NoobAdminBootstrapApplication : NoobAdminApplication
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void Application_Start(object sender, EventArgs e)
        {
            InitContainer();
        }

        protected override void Application_End(object sender, EventArgs e)
        {
            //InstanceNameRegistry.RemovePerformanceCounterInstances();   
            if (webConfig.PerformanceEnabled)
            {
                PerformanceMetricFactory.CleanupPerformanceMetrics();
            }
        }

        private WebConfig webConfig;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private WebConfig GetWebConfig()
        {
            string configPath = "~/App_Data/Configs/GeneralConfigs.config", sectionName = "GeneralConfigs/AdminConfig";
            return WebConfig.GetInstance(configPath, sectionName);
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual void InitContainer()
        {
            webConfig = GetWebConfig();
            InitApplicationStart();
            InitLogConfig();

            ContainerBuilder builder = ApplicationStarter.CreateHostBuilder();
            builder.RegisterModule(new NoobAdminBootstrapModule());

            builder.RegisterModule(new NLogModule());
            Register(builder);

            builder.RegisterInstance(webConfig).SingleInstance();

            IContainer container = builder.Build();

            //将当前容器中的控制器工厂替换掉MVC默认的控制器工厂。（即：不要MVC默认的控制器工厂了，用AutoFac容器中的控制器工厂替代）此处使用的是将AutoFac工作容器交给MVC底层 (需要using System.Web.Mvc;)  
            var resolver = new AutofacDependencyResolver(container);
            DependencyResolver.SetResolver(resolver);

            ContainerContext.Current.Container = container;

            
        }
       
        /// <summary>
        /// 
        /// </summary>
        public override void InitApplicationStart()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //使用Metrics监控应用程序的性能
            InitMetricConfig();

        }
        /// <summary>
        /// 使用Metrics监控应用程序的性能
        /// </summary>
        private void InitMetricConfig()
        {
            if (!webConfig.PerformanceEnabled)
            {
                return;
            }
            //string metricUrl = webConfig.WebUrl+webConfig.RootUrl+ "Metrics/";
            string metricUrl = webConfig.MetricUrl; //"http://127.0.0.1:1235/Metrics/";
            string metricsPath = HttpContext.Current.Server.MapPath("~/Logs/Metrics");
            Metric.Config
               .WithHttpEndpoint(metricUrl)
               .WithAllCounters()
               .WithReporting(config => config.WithCSVReports(metricsPath + @"\temp\csv", TimeSpan.FromSeconds(10))
               .WithTextFileReport(metricsPath + @"\temp\reports\metrics.txt", TimeSpan.FromSeconds(10)));
        }
        /// <summary>
        /// 
        /// </summary>
        private void InitLogConfig()
        {
            //var logConfig = Utils.GetMapPath("~/App_Data/Configs/NLog.config");
            //NLog.LogManager.Configuration = new XmlLoggingConfiguration(logConfig);
        }
    }
}