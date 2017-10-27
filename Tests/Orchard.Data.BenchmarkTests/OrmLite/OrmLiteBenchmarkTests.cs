using BenchmarkDotNet.Attributes;
using OrchardNorthwind.Data.OrmLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Data.BenchmarkTests.OrmLite
{
    /// <summary>
    /// 
    /// </summary>
    public class OrmLiteBenchmarkTests : BenchmarkTestBase
    {
        List<OrmLiteScenarioBase> GetOrmLiteSelectUseCases()
        {
            return new List<OrmLiteScenarioBase>
            {
				//new SelectOneModelWithFieldsOfDifferentTypesPerfScenario(),
				//new SelectOneSampleOrderLineScenario(),
				//new SelectManyModelWithFieldsOfDifferentTypesPerfScenario(),
				//new SelectManySampleOrderLineScenario(),
                new  SelectNorthwindDataScenario(),
            };
        }

        List<OrmLiteScenarioBase> GetOrmLiteInsertUseCases()
        {
            return new List<OrmLiteScenarioBase>
            {
				//new InsertModelWithFieldsOfDifferentTypesPerfScenario(),
				//new InsertSampleOrderLineScenario(),
                new InsertNorthwindDataScenario(),
            };
        }
        /// <summary>
        /// 
        /// </summary>
        [Benchmark]
        public void RunSelectTest()
        {
            foreach (var configRun in OrmLiteScenrioConfig.DataProviderConfigRuns())
            {
                RunBatchSelect(configRun);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configRun"></param>
        private  void RunBatchSelect(OrmLiteConfigRun configRun)
        {
            var useCases = GetOrmLiteSelectUseCases();
            BatchIterations.ForEach(iterations =>
            {
                useCases.ForEach(uc =>
                {
                    configRun.Init(uc);
                    //warmup
                    uc.Run();
                    //GC.Collect();
                });
            });
        }

        /// <summary>
        /// 
        /// </summary>
        [Benchmark]
        public void RunInsertTest()
        {
            foreach (var configRun in OrmLiteScenrioConfig.DataProviderConfigRuns())
            {
                RunBatchInsert(configRun);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configRun"></param>
        private void RunBatchInsert(OrmLiteConfigRun configRun)
        {
            var useCases = GetOrmLiteInsertUseCases();
            BatchIterations.ForEach(iterations =>
            {
                useCases.ForEach(uc =>
                {
                    configRun.Init(uc);
                    //warmup
                    uc.Run();
                    //GC.Collect();
                });
            });
        }
    }
}
