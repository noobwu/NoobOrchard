using NUnit.Framework;
using Orchard.Logging;
using Orchard.Logging.Log4Net;

namespace ServiceStack.Logging.Tests.UseCases
{
    [TestFixture]
    public class UsingLog4Net
    {
        [Test]
        public void Log4NetUseCase()
        {
            LoggerManager.LogFactory = new Log4NetFactory();
            ILogger log = LoggerManager.GetLogger(GetType());

            log.Debug("Debug Event Log Entry.");
            log.Warn("Warning Event Log Entry.");
        }
    }
}