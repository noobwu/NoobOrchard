using System;
using Autofac.Configuration.Core;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using Autofac;
namespace Orchard.Tests.Environment.AutofacUtil.Configuration.Core
{
    [TestFixture]
    public class ConfigurationRegistrarFixture : ConfigFixtureBase
    {
        [Test]
        public void RegisterConfiguration_NullBuilder()
        {
            var configuration = new Mock<IConfiguration>();
            var registrar = new ConfigurationRegistrar();
            Assert.Throws<ArgumentNullException>(() => registrar.RegisterConfiguration(null, configuration.Object));
        }

        [Test]
        public void RegisterConfiguration_NullConfiguration()
        {
            var builder = new ContainerBuilder();
            var registrar = new ConfigurationRegistrar();
            Assert.Throws<ArgumentNullException>(() => registrar.RegisterConfiguration(builder, null));
        }
    }
}
