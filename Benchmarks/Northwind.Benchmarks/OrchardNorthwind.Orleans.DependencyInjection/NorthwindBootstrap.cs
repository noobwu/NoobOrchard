using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NoobOrleans.Core;
using Orchard.Environment;
using Orchard.Logging.NLogger;
using OrchardNorthwind.IServices;
using OrchardNorthwind.Services.OrmLite;
using ServiceStack.OrmLite;
using System;
using System.Configuration;
namespace OrchardNorthwind.Orleans.DependencyInjection
{
    public class NorthwindBootstrap : OrleansBootstrap
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public override IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //services.AddSingleton<ILoggerFactory, LoggerFactory>();
            #region ContainerContext
            var builder = CreateBuilder();
            builder.Populate(services);
            var container = builder.Build();
            var containerContext = new ContainerContext
            {
                Container = container
            };
            services.AddSingleton(containerContext);

            #endregion ContainerContext

            return base.ConfigureServices(services);

        }
        private void ConfigBuilder(ContainerBuilder builder)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["NorthwindTest"].ConnectionString;
            var dbFactory = new OrmLiteConnectionFactory(connectionString, SqlServer2014Dialect.Provider);
            ApplicationStarter.InitBuilder(builder);
            builder.Register(c => dbFactory).InstancePerDependency();
            builder.RegisterModule(new NLogModule());
            builder.RegisterModule(new NorthwindOrmLiteModule(true));//
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private ContainerBuilder CreateBuilder()
        {
            var connectionString =ConfigurationManager.ConnectionStrings["NorthwindTest"].ConnectionString;
            var dbFactory = new OrmLiteConnectionFactory(connectionString, SqlServer2014Dialect.Provider);
            ContainerBuilder builder = ApplicationStarter.CreateHostBuilder();
            builder.Register(c => dbFactory).InstancePerDependency();
            builder.RegisterModule(new NLogModule());
            builder.RegisterModule(new NorthwindOrmLiteModule(true));//
            RegiserOrmLiteServices(builder);
            return builder;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        private void RegiserOrmLiteServices(ContainerBuilder builder)
        {
            builder.RegisterType<CategoryService>().As<ICategoryService>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerCustomerDemoService>().As<ICustomerCustomerDemoService>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerDemographicService>().As<ICustomerDemographicService>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerService>().As<ICustomerService>().InstancePerLifetimeScope();
            builder.RegisterType<EmployeeService>().As<IEmployeeService>().InstancePerLifetimeScope();
            builder.RegisterType<EmployeeTerritoryService>().As<IEmployeeTerritoryService>().InstancePerLifetimeScope();
            builder.RegisterType<OrderDetailService>().As<IOrderDetailService>().InstancePerLifetimeScope();
            builder.RegisterType<OrderService>().As<IOrderService>().InstancePerLifetimeScope();
            builder.RegisterType<ProductService>().As<IProductService>().InstancePerLifetimeScope();
            builder.RegisterType<RegionService>().As<IRegionService>().InstancePerLifetimeScope();
            builder.RegisterType<ShipperService>().As<IShipperService>().InstancePerLifetimeScope();
            builder.RegisterType<SupplierService>().As<ISupplierService>().InstancePerLifetimeScope();
            builder.RegisterType<TerritoryService>().As<ITerritoryService>().InstancePerLifetimeScope();
        }
    }
}
