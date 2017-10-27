using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using System.Configuration;
using Orchard.Utility;

namespace Orchard.Data.Storage.BenchmarkTests
{
    /// <summary>
    /// 
    /// </summary>
     //[OrderProvider(SummaryOrderPolicy.FastestToSlowest)] 多个Benchmark报错
    //[RankColumn] 多个Benchmark报错
    [Config(typeof(Config))]
    public abstract class BenchmarkBase
    {
        /// <summary>
        /// 
        /// </summary>
        public const int OperationsPerInvoke = 2;
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
        protected static int ParallelToExclusive = ConfigurationManager.AppSettings["ParallelToExclusive"].To<int>(10);

        /// 
        /// </summary>
        protected const int PageIndex = 1;
        /// <summary>
        /// 
        /// </summary>
        protected const int PageSize = 20;


        protected int i;

        protected virtual void BaseSetup()
        {
            i = 0;
        }

        protected virtual void Step()
        {
            i++;
            if (i > 5000) i = 1;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual Post CreatePost()
        {
            Post entity = new Post()
            {
                Text = RandomData.GetSentence(3,400),//Text
                CreationDate =DateTime.Now,//CreationDate
                LastChangeDate = RandomData.GetDateTime(),//LastChangeDate
                Counter1 = RandomData.GetInt(),//Counter1
                Counter2 = RandomData.GetInt(),//Counter2
                Counter3 = RandomData.GetInt(),//Counter3
                Counter4 = RandomData.GetInt(),//Counter4
                Counter5 = RandomData.GetInt(),//Counter5
                Counter6 = RandomData.GetInt(),//Counter6
                Counter7 = RandomData.GetInt(),//Counter7
                Counter8 = RandomData.GetInt(),//Counter8
                Counter9 = RandomData.GetInt(),//Counter9
            };
            return entity;
        }

    }
    public class Config : ManualConfig
    {
        //WithUnrollFactor():在生成的循环中每次迭代调用基准方法的次数是多少次
        //WithLaunchCount():我们应该在目标基准测试中启动多少次
        //WithWarmupCount():应该执行多少个预热迭代
        //WithIterationTime():单个迭代的期望时间
        //WithRemoveOutliers(true):所有的异常值都将从结果度量中删除
        //WithInvocationCount()在单个迭代中调用的计数(如果指定，迭代时间将被忽略)，必须是一个(UnrollFactor)的倍数\
        //WithTargetCount():应该执行多少个目标迭代
        public Config()
        {
            Add(new MemoryDiagnoser());
            Add(new StorageColum());
            //Add(new ReturnColum());
            Add(Job.Default
                .WithUnrollFactor(BenchmarkBase.Iterations)//
                //.WithIterationTime(new TimeInterval(500, TimeUnit.Millisecond))
                .WithLaunchCount(1)
                .WithWarmupCount(0)
                .WithTargetCount(5)
                .WithInvocationCount(BenchmarkBase.Iterations)
                .WithRemoveOutliers(true)
            );
        }
    }
}
