using NUnit.Framework;

namespace Orchard.Logging.Tests.UseCases
{
    [TestFixture]
    public class UsingDiagnosticsLogger
    {
        [Test]
        public void EventLogUseCase()
        {
            ILogger log = new DiagnosticsLogger("Orchard.Logging.Tests", "Application");

            log.Debug("Start Logging...");
        }
    }
}