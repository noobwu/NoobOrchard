using BenchmarkDotNet.Running;
using Orchard.Data.BenchmarkTests.EntityFramework;
using Orchard.Data.BenchmarkTests.EntityFrameworkCore;
using Orchard.Data.BenchmarkTests.OrmLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Data.BenchmarkTests
{
    class Program
    {
        static void Main(string[] args)
        {
            //BenchmarkSwitcher.FromAssembly(typeof(Program).GetTypeInfo().Assembly).Run(args);
            //var summary = BenchmarkRunner.Run<OrmLiteBenchmarkTests>();
            var switcher = new BenchmarkSwitcher(new[] {
                typeof(OrmLiteBenchmarkTests),
                typeof(EfBenchmarkTests),
                typeof(EfCoreBenchmarkTests)
            });
            switcher.Run(args);
        }
    }
}
