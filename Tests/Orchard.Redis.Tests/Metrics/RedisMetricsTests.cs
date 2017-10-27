using Orchard.Metrics;
using Orchard.Redis.Metrics;
using Orchard.Tests.Metrics;
using System;
using System.Threading.Tasks;
using NUnit.Framework;
namespace Orchard.Redis.Tests.Metrics
{
    public class RedisMetricsTests : MetricsClientTestBase, IDisposable {
        public RedisMetricsTests(){
            var muxer = SharedConnection.GetMuxer();
            muxer.FlushAllAsync().GetAwaiter().GetResult();
        }

        public override IMetricsClient GetMetricsClient(bool buffered = false) {
            return new RedisMetricsClient(new RedisMetricsClientOptions { ConnectionMultiplexer = SharedConnection.GetMuxer(), Buffered = buffered, LoggerFactory = LogFactory });
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
            return base.CanWaitForCounterAsync();
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

        public void Dispose() {
            var muxer = SharedConnection.GetMuxer();
            muxer.FlushAllAsync().GetAwaiter().GetResult();
        }
    }
}