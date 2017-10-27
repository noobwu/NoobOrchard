using BenchmarkDotNet.Running;
using OrchardNorthwind.Services.OrleansClient.Benchmarks;
using OrchardNorthwind.Services.OrleansClient.Tests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace OrchardNorthwind.Services.OrleansClient
{
    /// <summary>
    /// 
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            string logPath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }
            try
            {
                Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadKey();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private static void Run()
        {
            //RunTopshelf();
            RunBenchmarks();
        }
        private static void RunBenchmarks()
        {
            var summary = BenchmarkRunner.Run<CategoryGrainBenchmark>();
        }
        /// <summary>
        /// 
        /// </summary>
        private static void RunTopshelf()
        {
            //安装服务  OrchardNorthwind.Services.OrleansClient.exe install
            //卸载服务  OrchardNorthwind.Services.OrleansClient.exe uninstall
            //启动服务  OrchardNorthwind.Services.OrleansClient.exe start
            //停止服务  OrchardNorthwind.Services.OrleansClient.exe stop
            HostFactory.Run(x =>
            {
                x.RunAsLocalSystem();

                x.SetServiceName("OrchardNorthwindServicesOrleansClient");
                x.SetDisplayName("OrchardNorthwindServicesOrleans Topshelf Client");
                x.SetDescription("using topshelf to  orleans client,processing service logic etc.");
                x.SetStartTimeout(TimeSpan.FromMinutes(5));
                //https://github.com/Topshelf/Topshelf/issues/165
                x.SetStopTimeout(TimeSpan.FromMinutes(35));

                x.EnableServiceRecovery(r => { r.RestartService(1); });

                x.Service(factory => {
                    return new SiloClientService(() => { RunGrainTests(); });
                });
            });
        }
        /// <summary>
        /// 
        /// </summary>
        private static void RunGrainTests()
        {
            Console.WriteLine("\nPress Enter to Run NorthwindGrainTests");
            Console.ReadLine();
            try
            {
                Console.WriteLine("CategoryGrainTests Starting");
                var categoryGrainTests = new CategoryGrainTests();
                Console.WriteLine("CountAsync():" + categoryGrainTests.CountAsync().Result);
                Console.WriteLine("CountAsyncByPredicate():" + categoryGrainTests.CountAsyncByPredicate().Result);
                Console.WriteLine("LongCountAsync():" + categoryGrainTests.LongCountAsync().Result);
                Console.WriteLine("LongCountAsyncByPredicate():" + categoryGrainTests.LongCountAsyncByPredicate().Result);
                Console.WriteLine("SingleAsync():" + categoryGrainTests.SingleAsync().Result);
                Console.WriteLine("SingleAsyncByPredicate():" + categoryGrainTests.SingleAsyncByPredicate().Result);
                Console.WriteLine("CategoryGrainTests Ending");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }
    }
}
