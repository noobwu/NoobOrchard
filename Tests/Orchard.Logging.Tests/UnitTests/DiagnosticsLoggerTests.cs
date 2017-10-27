using System;
using NUnit.Framework;

namespace Orchard.Logging.Tests.UnitTests
{
    [TestFixture]
    public class DiagnosticsLoggerTests
    {
        [Test]
        public void DiagnosticsLogger()
        {
            ILogger log = new DiagnosticsLogger("Orchard.Logging.Tests", "Application");
            Assert.IsNotNull(log);
        }

        [Test]
        public void DiagnosticsLogger_NullLogNameTest()
        {
            Assert.Throws<ArgumentNullException>(() => {
                ILogger log = new DiagnosticsLogger(null, "Application");
            });
        }

        [Test]
        public void DiagnosticsLogger_NullSourceNameTest()
        {
            Assert.Throws<ArgumentNullException>(() => {
                ILogger log = new DiagnosticsLogger("Orchard.Logging.Tests", null);
            });
        }

        [Test]
        public void DiagnosticsLogger_LoggingTest()
        {
            string message = "Error Message";
            Exception ex = new Exception("Exception");
            string messageFormat = "Message Format: message: {0}, exception: {1}";

            ILogger log = new DiagnosticsLogger("Orchard.Logging.Tests", "Application");
            Assert.IsNotNull(log);

            log.Debug(message);
            log.Debug(message, ex);
            log.DebugFormat(messageFormat, message, ex.Message);

            log.Error(message);
            log.Error(message, ex);
            log.ErrorFormat(messageFormat, message, ex.Message);

            log.Fatal(message);
            log.Fatal(message, ex);
            log.FatalFormat(messageFormat, message, ex.Message);

            log.Info(message);
            log.Info(message, ex);
            log.InfoFormat(messageFormat, message, ex.Message);

            log.Warn(message);
            log.Warn(message, ex);
            log.WarnFormat(messageFormat, message, ex.Message);
        }
    }
}