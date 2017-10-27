using Autofac;
using BenchmarkDotNet.Attributes;
using System.Configuration;

namespace Orchard.Repositories.BenchmarkTests
{
    /// <summary>
    /// 
    /// </summary>
    //[OrderProvider(SummaryOrderPolicy.FastestToSlowest)] 多个Benchmark报错
    //[RankColumn] 多个Benchmark报错
    [Config(typeof(Config))]
    public abstract class ParallelBenchmarkBase
    {
        /// <summary>
        /// 
        /// </summary>
        public static int OperationsPerInvoke = ConfigurationManager.AppSettings["OperationsPerInvoke"].To<int>(5);
        /// <summary>
        /// 
        /// </summary>
        public static int Iterations = ConfigurationManager.AppSettings["Iterations"].To<int>(5);
        /// <summary>
        /// Parallel开始索引（含）。
        /// </summary>
        protected static int ParallelFromExclusive = ConfigurationManager.AppSettings["ParallelFromExclusive"].To<int>(0);
        /// <summary>
        /// Parallel结束索引（不含）。
        /// </summary>
        protected static int ParallelToExclusive = ConfigurationManager.AppSettings["ParallelToExclusive"].To<int>(100);
        /// <summary>
        /// 
        /// </summary>
        protected const int PageIndex = 1;
        /// <summary>
        /// 
        /// </summary>
        protected const int PageSize = 20;
        protected Autofac.IContainer Container;

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
}
