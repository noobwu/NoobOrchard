using Orchard.Metrics;
using System.Threading.Tasks;
using NUnit.Framework;
using Orchard.Utility;

namespace Orchard.Tests.Metrics {
    public class InMemoryMetricsTests : MetricsClientTestBase {

        public override IMetricsClient GetMetricsClient(bool buffered = false) {
            return new InMemoryMetricsClient(new InMemoryMetricsClientOptions { LoggerFactory = LogFactory, Buffered = buffered });
        }

        [Test]
        public override Task CanSetGaugesAsync() {
            return base.CanSetGaugesAsync();
        }

        [Test]
        public override Task CanIncrementCounterAsync() {
            return base.CanIncrementCounterAsync();
        }

        [Test]
        public override Task CanWaitForCounterAsync() {
            using (TestSystemClock.Install()) {
                return base.CanWaitForCounterAsync();
            }
        }

        [Test]
        public override Task CanGetBufferedQueueMetricsAsync() {
            return base.CanGetBufferedQueueMetricsAsync();
        }

        [Test]
        public override Task CanIncrementBufferedCounterAsync() {
            return base.CanIncrementBufferedCounterAsync();
        }

        [Test]
        public override Task CanSendBufferedMetricsAsync() {
            return base.CanSendBufferedMetricsAsync();
        }
    }
}