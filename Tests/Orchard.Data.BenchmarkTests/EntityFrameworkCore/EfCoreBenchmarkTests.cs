using BenchmarkDotNet.Attributes;
using OrchardNorthwind.Data.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Data.BenchmarkTests.EntityFrameworkCore
{
    public class EfCoreBenchmarkTests : BenchmarkTestBase
    {
        private List<EfCoreScenarioBase> GetEfSelectCoreUseCases()
        {
            return new List<EfCoreScenarioBase>
            {
                new  EfCoreInsertNorthwindDataScenario(),
            };
        }
        private List<EfCoreScenarioBase> GetEfInsertCoreUseCases()
        {
            return new List<EfCoreScenarioBase>
            {
                new  EfCoreInsertNorthwindDataScenario(),
            };
        }

        [Benchmark]
        public void RunSelectTest()
        {
            foreach (var configRun in EfCoreScenrioConfig.DbContextConfigRuns())
            {
                RunBatchSelect(configRun);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="args"></param>
        private void RunBatchSelect(Microsoft.EntityFrameworkCore.DbContext dbContext)
        {
            BatchIterations.ForEach(iterations =>
            {
                var useCases = GetEfSelectCoreUseCases();
                useCases.ForEach(uc =>
                {
                    uc.Context = dbContext;
                    // warmup
                    uc.Run();
                    //GC.Collect();
                });
            });
        }

        [Benchmark]
        public void RunInsertTest()
        {
            foreach (var configRun in EfCoreScenrioConfig.DbContextConfigRuns())
            {
                RunBatchInsert(configRun);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="args"></param>
        private void RunBatchInsert(Microsoft.EntityFrameworkCore.DbContext dbContext)
        {
            BatchIterations.ForEach(iterations =>
            {
                var useCases = GetEfInsertCoreUseCases();
                useCases.ForEach(uc =>
                {
                    uc.Context = dbContext;
                    // warmup
                    uc.Run();
                    //GC.Collect();
                });
            });
        }
    }
}
