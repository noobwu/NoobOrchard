using Orchard.Metrics;
using Orchard.Tests.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Orchard.Utility;

namespace Orchard.Tests.Metrics
{
    public class StatsDMetricsTests : TestWithLoggingBase, IDisposable
    {
        private readonly int _port = new Random(12345).Next(10000, 15000);
        private readonly StatsDMetricsClient _client;
        private readonly UdpListener _listener;
        private Thread _listenerThread;

        public StatsDMetricsTests()
        {
            _listener = new UdpListener("127.0.0.1", _port);
            _client = new StatsDMetricsClient(new StatsDMetricsClientOptions { ServerName = "127.0.0.1", Port = _port, Prefix = "test", LoggerFactory = LogFactory });
        }

        [Test]
        public async Task CounterAsync()
        {
            await StartListeningAsync(1);
            await _client.CounterAsync("counter");
            var messages = GetMessages();
            Assert.AreEqual("test.counter:1|c", messages.FirstOrDefault());
        }

        [Test]
        public async Task CounterAsyncWithValue()
        {
            await StartListeningAsync(1);

            await _client.CounterAsync("counter", 5);
            var messages = GetMessages();
            Assert.AreEqual("test.counter:5|c", messages.FirstOrDefault());
        }

        [Test]
        public async Task GaugeAsync()
        {
            await StartListeningAsync(1);

            await _client.GaugeAsync("gauge", 1.1);
            var messages = GetMessages();
            Assert.AreEqual("test.gauge:1.1|g", messages.FirstOrDefault());
        }

        [Test]
        public async Task TimerAsync()
        {
            await StartListeningAsync(1);

            await _client.TimerAsync("timer", 1);
            var messages = GetMessages();
            Assert.AreEqual("test.timer:1|ms", messages.FirstOrDefault());
        }

        [Test]
        public async Task CanSendOffline()
        {
            await _client.CounterAsync("counter");
            var messages = GetMessages();
            Assert.IsEmpty(messages);
        }

        [Test]
        public async Task CanSendMultithreaded()
        {
            const int iterations = 100;
            await StartListeningAsync(iterations);

            await Run.InParallelAsync(iterations, async i =>
            {
                await SystemClock.SleepAsync(50);
                await _client.CounterAsync("counter");
            });

            var messages = GetMessages();
            Assert.AreEqual(iterations, messages.Count);
        }

        [Ignore("Flakey")]
        public async Task CanSendMultiple()
        {
            const int iterations = 100000;
            await StartListeningAsync(iterations);

            var metrics = new InMemoryMetricsClient(new InMemoryMetricsClientOptions());

            var sw = Stopwatch.StartNew();
            for (int index = 0; index < iterations; index++)
            {
                if (index % (iterations / 10) == 0)
                    StopListening();

                await _client.CounterAsync("counter");
                await metrics.CounterAsync("counter");

                if (index % (iterations / 10) == 0)
                    await StartListeningAsync(iterations - index);

                if (index % (iterations / 20) == 0)
                    _logger.Debug((await metrics.GetCounterStatsAsync("counter")).ToString());
            }

            sw.Stop();
            _logger.Info((await metrics.GetCounterStatsAsync("counter")).ToString());

            // Require at least 10,000 operations/s
            Assert.LessOrEqual(sw.ElapsedMilliseconds, (iterations / 10000.0) * 1000);

            await SystemClock.SleepAsync(250);
            var messages = GetMessages();
            int expected = iterations - (iterations / (iterations / 10));
            Assert.IsTrue(messages.Count >= expected - 10 && messages.Count <= expected + 10);
            foreach (string message in messages)
                Assert.AreEqual("test.counter:1|c", message);
        }

        private List<string> GetMessages()
        {
            while (_listenerThread != null && _listenerThread.IsAlive) { }

            return _listener.GetMessages();
        }

        private Task StartListeningAsync(int expectedMessageCount)
        {
            _listenerThread = new Thread(_listener.StartListening) { IsBackground = true };
            _listenerThread.Start(expectedMessageCount);

            return SystemClock.SleepAsync(75);
        }

        private void StopListening()
        {
            _listenerThread.Abort();
        }

        public void Dispose()
        {
            _listener.Dispose();
        }
    }
}