using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static System.Console;
namespace Orchard.Data.Storage.BenchmarkTests
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            WriteLineColor("Warning: DEBUG configuration; performance may be impacted!", ConsoleColor.Red);
            WriteLine();
#endif
            WriteLine("Welcome to data storage performance benchmark suite, based on BenchmarkDotNet.");
            if (args.Length == 0)
            {
                WriteLine("Optional arguments:");
                WriteColor("  --all", ConsoleColor.Blue);
                WriteLine(": run all benchmarks");
                WriteColor("  --parallel", ConsoleColor.Blue);
                WriteLine(": run all parallel benchmarks", ConsoleColor.Gray);
                WriteLine();
            }
            WriteLine("Iterations: " + BenchmarkBase.Iterations);
            if (args.Any(a => a == "--all"))
            {
                //运行所有的测试
                RunAllBenchmark(args);
            }
            if (args.Any(a => a == "--parallel"))
            {
                //运行所有并发测试
                RunAllParallelBenchmark(args);
            }
            else
            {
                //运行选择的测试
                RunBenchmarkSwitcher(args);
            }
          
        }

        /// <summary>
        /// 运行所选的测试
        /// </summary>
        /// <param name="args"></param>
        private static void RunBenchmarkSwitcher(string[] args)
        {
            //BenchmarkSwitcher.FromAssembly(typeof(Program).GetTypeInfo().Assembly).Run(args);
            //var summary = BenchmarkRunner.Run<SqlServerBenchmarks>();
            var benchTypes = Assembly.GetEntryAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(BenchmarkBase)));
            if (benchTypes == null || benchTypes.Count() == 0)
            {
                WriteLineColor("There is no benchmarks suite", ConsoleColor.Red);
            }
            else
            {
                var switcher = new BenchmarkSwitcher(benchTypes.ToArray());
                try
                {
                    switcher.Run(args);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Console.ReadKey();
                }
            }
  
        }
        /// <summary>
        ///  运行所有的测试
        /// </summary>
        /// <param name="args"></param>
        private static void RunAllBenchmark(string[] args)
        {
           
            var benchTypes = Assembly.GetEntryAssembly().DefinedTypes.Where(t => t.IsSubclassOf(typeof(BenchmarkBase)));
            if (benchTypes == null || benchTypes.Count() == 0)
            {
                WriteLineColor("There is no benchmarks suite", ConsoleColor.Red);
            }
            else
            {
                var benchmarks = new List<Benchmark>();
                WriteLineColor("Running full benchmarks suite", ConsoleColor.Green);
                foreach (var b in benchTypes)
                {
                    benchmarks.AddRange(BenchmarkConverter.TypeToBenchmarks(b));
                }
                try
                {
                    BenchmarkRunner.Run(benchmarks.ToArray(), null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Console.ReadKey();
                }
            }
        }
        /// <summary>
        ///  运行所有并发测试
        /// </summary>
        /// <param name="args"></param>
        private static void RunAllParallelBenchmark(string[] args)
        {
            var benchmarks = new List<Benchmark>();
            var benchTypes = Assembly.GetEntryAssembly().DefinedTypes.Where(t => t.IsSubclassOf(typeof(ParallelBenchmarkBase)));
            WriteLineColor("Running full parallel benchmarks suite", ConsoleColor.Green);
            if (benchTypes == null || benchTypes.Count() == 0)
            {
                WriteLineColor("There is no parallel benchmarks suite", ConsoleColor.Red);
            }
            else
            {
                WriteLineColor("Running full parallel benchmarks suite", ConsoleColor.Green);
                foreach (var b in benchTypes)
                {
                    benchmarks.AddRange(BenchmarkConverter.TypeToBenchmarks(b));
                }
                try
                {
                    BenchmarkRunner.Run(benchmarks.ToArray(), null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Console.ReadKey();
                }
            }
        }
        public static void WriteLineColor(string message, ConsoleColor color)
        {
            var orig = ForegroundColor;
            ForegroundColor = color;
            WriteLine(message);
            ForegroundColor = orig;
        }

        public static void WriteColor(string message, ConsoleColor color)
        {
            var orig = ForegroundColor;
            ForegroundColor = color;
            Write(message);
            ForegroundColor = orig;
        }
    }
}
