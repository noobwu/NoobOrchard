using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Orchard.Benchmarks.Caching;
using Orchard.Benchmarks.Queues;
using System;

namespace Orchard.Benchmarks
{
    class Program {
        static void Main(string[] args) {
            var summary = BenchmarkRunner.Run<QueueBenchmarks>();
            Console.WriteLine(summary.ToString());

            summary = BenchmarkRunner.Run<CacheBenchmarks>();
            Console.WriteLine(summary.ToString());
            Console.ReadKey();
        }
    }

    class BenchmarkConfig : ManualConfig {
        public BenchmarkConfig() {
            Add(Job.Default.WithWarmupCount(1).WithTargetCount(1));
        }
    }
}