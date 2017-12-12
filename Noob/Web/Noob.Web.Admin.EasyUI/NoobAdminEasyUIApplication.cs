using Autofac;
using Autofac.Integration.Mvc;
using Orchard.Environment;
using Orchard.Logging.NLogger;
using Orchard.Web.Mvc.Infrastructure;
using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Noob.Web.Admin.EasyUI
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class NoobAdminEasyUIApplication : NoobAdminApplication
    {
        /// <summary>
        /// 
        /// </summary>
        public virtual void InitContainer()
        {
            InitApplicationStart();

            ContainerBuilder builder = ApplicationStarter.CreateHostBuilder();
            builder.RegisterModule(new NoobAdminEasyUIModule());

            builder.RegisterModule(new NLogModule());
            Register(builder);

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
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void Application_Start(object sender, EventArgs e)
        {
             InitContainer();
        }

    }
}