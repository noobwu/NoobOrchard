using Autofac;
using Autofac.Integration.Mvc;
using Metrics;
using Orchard.Environment;
using Orchard.Environment.Configuration;
using Orchard.Logging;
using Orchard.Logging.NLogger;
using Orchard.Performance;
using Orchard.Web.Mvc.Infrastructure;
using Orleans;
using Orleans.Runtime;
using System;
using System.Threading;
using System.Web;
using System.Web.Hosting;
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
        private ILogger logger;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Start(object sender, EventArgs e)
        {
            InitContainer();
            StartClient();
        }

        protected void Application_End()
        {
            //InstanceNameRegistry.RemovePerformanceCounterInstances();   
            if (webConfig.PerformanceEnabled)
            {
                PerformanceMetricFactory.CleanupPerformanceMetrics();
            }
            client?.Close();
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

            var loggerFactory = container.Resolve<ILoggerFactory>();
            if (loggerFactory != null)
            {
                logger = loggerFactory.GetLogger(this.GetType());
            }
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
            Metrics.Metric.Config
               .WithHttpEndpoint(metricUrl)
               .WithAllCounters()
               .WithReporting(config => config.WithCSVReports(metricsPath + @"\temp\csv", TimeSpan.FromSeconds(10))
               .WithTextFileReport(metricsPath + @"\temp\reports\metrics.txt", TimeSpan.FromSeconds(10)));
        }
        #region OrleansClient
        /// <summary>
        /// 
        /// </summary>
        private IClusterClient client;
        /// <summary>
        /// 
        /// </summary>
        private void StartClient()
        {
            Orleans.Runtime.Configuration.ClientConfiguration clientConfig = null;
            //bool startOk = false;
            try
            {
                string configPath = HostingEnvironment.MapPath("~/App_Data/Configs/ClientConfiguration.xml");
                clientConfig = Orleans.Runtime.Configuration.ClientConfiguration.LoadFromFile(configPath);
            }
            catch (Exception ex)
            {
                if (logger != null)
                {
                    logger.Error("LoadFromFile Error", ex);
                }
                return;
            }

            try
            {
                //clientConfig.SerializationProviders.Add(typeof(LinqSerializer).GetTypeInfo());
                InitializeWithRetries(clientConfig, initializeAttemptsBeforeFailing: 5);
            }
            catch (Exception ex)
            {
                if (logger != null)
                {
                    logger.Error($"Orleans client initialization failed failed due",ex);
                }
                return;
            }
            try
            {
                // Then configure and connect a client.

                //client = new ClientBuilder().UseConfiguration(clientConfig).Build();
                //client.Connect().Wait();
                if (logger != null)
                {
                    logger.Debug("Client connected.");
                }
                //startOk = true;
                //
                // This is the place for your test code.

            }
            catch (Exception ex)
            {
                if (logger != null)
                {
                    logger.Error($"Orleans client Connect failed", ex);
                }
            }
            //return startOk;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="initializeAttemptsBeforeFailing"></param>
        private void InitializeWithRetries(Orleans.Runtime.Configuration.ClientConfiguration config, int initializeAttemptsBeforeFailing)
        {
            int attempt = 0;
            while (true)
            {
                try
                {
                    GrainClient.Initialize(config);
                    client = GrainClient.Instance;
                    if (logger != null)
                    {
                        logger.Debug("Client successfully connect to silo host");
                    }
                    break;
                }
                catch (SiloUnavailableException ex)
                {
                    attempt++;
                    if (logger != null)
                    {
                        logger.Error($"Attempt {attempt} of {initializeAttemptsBeforeFailing} failed to initialize the Orleans client.", ex);
                    }
                    if (attempt > initializeAttemptsBeforeFailing)
                    {
                        throw ex;
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(2));
                }
                catch (Exception ex)
                {
                    if (logger != null)
                    {
                        logger.Error($"Attempt {attempt} of {initializeAttemptsBeforeFailing} failed to initialize the Orleans client.");
                    }
                    throw ex;
                }
            }
        }
        #endregion OrleansClient
    }
}