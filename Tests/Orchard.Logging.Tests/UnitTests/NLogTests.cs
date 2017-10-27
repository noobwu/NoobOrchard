using System;
using NUnit.Framework;
using Orchard.Logging.NLogger;

namespace Orchard.Logging.Tests.UnitTests
{
    [TestFixture]
    public class NLogTests
    {
        [Test]
        public void Does_maintain_callsite()
        {
            LoggerManager.LogFactory  = new NLogFactory();
            var log = LoggerManager.LogFactory.GetLogger(GetType());
            log.InfoFormat("Message");
            log.InfoFormat("Message with Args {0}", "Foo");
            log.Info("Message with Exception", new Exception("Foo Exception"));
        }
    }
}