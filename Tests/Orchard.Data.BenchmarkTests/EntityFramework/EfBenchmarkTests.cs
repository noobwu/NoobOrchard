using BenchmarkDotNet.Attributes;
using OrchardNorthwind.Data.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Data.BenchmarkTests.EntityFramework
{
    /// <summary>
    /// 
    /// </summary>
    public class EfBenchmarkTests : BenchmarkTestBase
    {
        private List<EfScenarioBase> GetEfInsertUseCases()
        {
            return new List<EfScenarioBase>
            {
                new  EfInsertNorthwindDataScenario()
            };
        }
        private List<EfScenarioBase> GetEfSelectUseCases()
        {
            return new List<EfScenarioBase>
            {
                new  EfSelectNorthwindDataScenario(),
            };
        }
        [Benchmark]
        public void RunSelectTest()
        {
            foreach (var configRun in EfScenrioConfig.DbContextConfigRuns())
            {
                RunBatchSelect(configRun);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="args"></param>
        private void RunBatchSelect(System.Data.Entity.DbContext dbContext)
        {
            BatchIterations.ForEach(iterations =>
            {
                var useCases = GetEfSelectUseCases();
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
            foreach (var configRun in EfScenrioConfig.DbContextConfigRuns())
            {
                RunBatchInsert(configRun);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="args"></param>
        private void RunBatchInsert(System.Data.Entity.DbContext dbContext)
        {
            BatchIterations.ForEach(iterations =>
            {
                var useCases = GetEfInsertUseCases();
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
