using Autofac;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.Environment;
using Orchard.Environment.Configuration;
using Orchard.Services;
using Orchard.Caching.Services;

namespace Orchard.Caching.Tests
{
    [TestFixture]
    public abstract class CachingTestsBase
    {
        /// <summary>
        /// 
        /// </summary>
        protected IContainer Container;
        [OneTimeSetUp]
        public void InitFixture()
        {
        }

        [OneTimeTearDown]
        public void TearDownFixture()
        {

        }

        [SetUp]
        public virtual void Init()
        {
            ShellSettings shellSettings = new ShellSettings() {
                Name= "Default"
            };
            var builder = new ContainerBuilder();
            builder.RegisterInstance(shellSettings).SingleInstance();
            builder.RegisterType<Clock>().As<IClock>().SingleInstance();
            builder.RegisterType<DefaultCacheStorageProvider>().As<ICacheStorageProvider>().SingleInstance();
            builder.RegisterType<DefaultCacheService>().As<ICacheService>().SingleInstance();
            Register(builder);
            Container = builder.Build();
        }
        public abstract void Register(ContainerBuilder builder);
        [TearDown]
        public void Cleanup()
        {
            if (Container != null)
            {
                Container.Dispose();
            }
        }


    }
}
