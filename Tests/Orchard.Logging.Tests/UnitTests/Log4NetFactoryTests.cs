using System.IO;
using NUnit.Framework;
using Orchard.Logging.Log4Net;

namespace Orchard.Logging.Tests.UnitTests
{
    [TestFixture]
    public class Log4NetFactoryTests
    {
        [Test]
        public void Log4NetFactoryTest()
        {
            Log4NetFactory factory = new Log4NetFactory();
            ILogger log = factory.GetLogger(GetType());
            Assert.IsNotNull(log);
            Assert.IsNotNull(log as Log4NetLogger);

            factory = new Log4NetFactory(true);
            log = factory.GetLogger(GetType().Name);
            Assert.IsNotNull(log);
            Assert.IsNotNull(log as Log4NetLogger);
        }

        [Test]
        public void Log4NetFactoryTestWithExistingConfigFile()
        {
            const string configFile = "log4net.Test.config";
            Assert.IsTrue(File.Exists(configFile), "Test setup failure. Required log4net config file is missing.");

            Log4NetFactory factory = new Log4NetFactory(configFile);

            ILogger log = factory.GetLogger(GetType());
            Assert.IsNotNull(log);
            Assert.IsNotNull(log as Log4NetLogger);
        }
    }
}