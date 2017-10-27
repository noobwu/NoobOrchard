using NUnit.Framework;
using Rhino.Mocks;

namespace Orchard.Logging.Tests.UnitTests
{
    [TestFixture]
    public class LogManagerTests : UnitTestBase
    {
        [Test]
        public void LogManager_DefaultTest()
        {
            ILogger log = LoggerManager.GetLogger(GetType());
            Assert.IsNotNull(log);
            Assert.IsNotNull(LoggerManager.LogFactory as NullLoggerFactory);
            Assert.IsNotNull(log as NullLogger);

            log = LoggerManager.GetLogger(GetType().Name);
            Assert.IsNotNull(log);
            Assert.IsNotNull(LoggerManager.LogFactory as NullLoggerFactory);
            Assert.IsNotNull(log as NullLogger);
        }

        [Test]
        public void LogManager_InjectionTest()
        {
            ILoggerFactory factory = Mocks.StrictMock<ILoggerFactory>();
            Expect.Call(factory.GetLogger(GetType())).Return(Mocks.DynamicMock<ILogger>());
            ReplayAll();

            LoggerManager.LogFactory = factory;
            ILogger log = LoggerManager.GetLogger(GetType());

            Assert.IsNotNull(log);
            VerifyAll();
        }
    }
}