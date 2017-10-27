using Autofac;
using Orchard.Caching;
using Orchard.Events;
using Orchard.Exceptions;
using Orchard.FileSystems.AppData;
using Orchard.Logging;
using Orchard.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Environment
{
   /// <summary>
   /// 
   /// </summary>
    public static class ApplicationStarter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="registrations"></param>
        /// <returns></returns>
        public static ContainerBuilder CreateHostBuilder()
        {
            var builder = new ContainerBuilder();
            InitBuilder(builder);
            return builder;
        }
        /// <summary>
        /// 
        /// </summary>
        public static void InitBuilder(ContainerBuilder builder)
        {
            builder.RegisterModule(new CollectionOrderModule());
            builder.RegisterModule(new EventsModule());
            builder.RegisterModule(new CacheModule());

            // a single default host implementation is needed for bootstrapping a web app domain
            builder.RegisterType<DefaultCacheHolder>().As<ICacheHolder>().SingleInstance();
            builder.RegisterType<DefaultCacheContextAccessor>().As<ICacheContextAccessor>().SingleInstance();
            builder.RegisterType<DefaultParallelCacheContext>().As<IParallelCacheContext>().SingleInstance();
            builder.RegisterType<DefaultExceptionPolicy>().As<IExceptionPolicy>().SingleInstance();
            //builder.RegisterType<NullLoggerFactory>().As<ILoggerFactory>().SingleInstance();

            //builder.RegisterType<DefaultOrchardEventBus>().As<IEventBus>().SingleInstance();
            //builder.RegisterType<DefaultAsyncTokenProvider>().As<IAsyncTokenProvider>().SingleInstance();
            //builder.RegisterType<AppDataFolderRoot>().As<IAppDataFolderRoot>().SingleInstance();

            //RegisterVolatileProvider<AppDataFolder, IAppDataFolder>(builder);
            //RegisterVolatileProvider<Clock, IClock>(builder);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TRegister"></typeparam>
        /// <typeparam name="TService"></typeparam>
        /// <param name="builder"></param>
        private static void RegisterVolatileProvider<TRegister, TService>(ContainerBuilder builder) where TService : IVolatileProvider
        {
            builder.RegisterType<TRegister>()
                .As<TService>()
                .As<IVolatileProvider>()
                .SingleInstance();
        }
    }
}
