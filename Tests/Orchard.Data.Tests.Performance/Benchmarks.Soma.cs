using BenchmarkDotNet.Attributes;

namespace Orchard.Data.Tests.Performance
{
    public class SomaBenchmarks : BenchmarkBase
    {
        private dynamic _sdb;

        [GlobalSetup]
        public void Setup()
        {
            BaseSetup();
            _sdb = Simple.Data.Database.OpenConnection(ConnectionString);
        }

        [Benchmark(Description = "FindById")]
        public dynamic QueryDynamic()
        {
            Step();
            return _sdb.Posts.FindById(i).FirstOrDefault();
        }
    }
}