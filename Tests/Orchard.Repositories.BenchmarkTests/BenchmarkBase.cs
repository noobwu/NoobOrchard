using Autofac;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using System.Configuration;

namespace Orchard.Repositories.BenchmarkTests
{
    /// <summary>
    /// 
    /// </summary>
     //[OrderProvider(SummaryOrderPolicy.FastestToSlowest)] 多个Benchmark报错
    //[RankColumn] 多个Benchmark报错
    [Config(typeof(Config))]
    public abstract  class BenchmarkBase
    {
        /// <summary>
        /// 
        /// </summary>
        public static int OperationsPerInvoke = ConfigurationManager.AppSettings["OperationsPerInvoke"].To<int>(2);
        /// <summary>
        /// 
        /// </summary>
        public static int Iterations = ConfigurationManager.AppSettings["Iterations"].To<int>(5);
     
        /// 
        /// </summary>
        protected const int PageIndex = 1;
        /// <summary>
        /// 
        /// </summary>
        protected const int PageSize = 20;
        protected IContainer Container;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        protected abstract void Register(ContainerBuilder builder);
        /// <summary>
        /// 
        /// </summary>
        public virtual void Cleanup()
        {
            Container?.Dispose();
        }
        public static string ConnectionString { get; } = ConfigurationManager.ConnectionStrings["Test"].ConnectionString;
    }
    public class Config : ManualConfig
    {
        public Config()
        {
            Add(new MemoryDiagnoser());
            Add(new ORMColum());
            //Add(new ReturnColum());
            Add(Job.Default
                .WithUnrollFactor(BenchmarkBase.Iterations)
                //.WithIterationTime(new TimeInterval(500, TimeUnit.Millisecond))
                .WithLaunchCount(1)
                .WithWarmupCount(0)
                .WithTargetCount(5)
                .WithRemoveOutliers(true)
            );
        }
    }
}
