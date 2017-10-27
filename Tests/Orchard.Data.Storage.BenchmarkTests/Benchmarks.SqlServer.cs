using BenchmarkDotNet.Attributes;
using Dapper.Contrib.Extensions;
using System.Linq;
using Dapper;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Orchard.Data.Storage.BenchmarkTests
{
    public class SqlServerBenchmarks : BenchmarkBase
    {
        protected SqlConnection _connection;
        public static string ConnectionString { get; } = ConfigurationManager.ConnectionStrings["Test"].ConnectionString;

        protected override void BaseSetup()
        {
            _connection = new SqlConnection(ConnectionString);
            _connection.Open();
            base.BaseSetup();
        }

        [GlobalSetup]
        public void Setup()
        {
            BaseSetup();
        }
        [GlobalCleanup]
        public void Cleanup()
        {
            _connection?.Dispose();
        }
        [Benchmark(Description = "Insert<T>")]
        public Post Insert()
        {
            Step();
            return InsertData();
        }

        [Benchmark(Description = "InsertAsync<T>")]
        public Task<Post> InsertAsync()
        {
            Step();
            return InsertDataAsync();
        }

        [Benchmark(Description = "Delete<T>")]
        public bool Delete()
        {
            Step();
            var entity = InsertData();
            return _connection.Delete(entity);
        }

        [Benchmark(Description = "DeleteAsync<T>")]
        public Task<bool> DeleteAsync()
        {
            Step();
            var entity = InsertData();
            return _connection.DeleteAsync(entity);
        }

        [Benchmark(Description = "Update<T>")]
        public bool Update()
        {
            Step();
            var entity = InsertData();
            return _connection.Update(entity);
        }

        [Benchmark(Description = "UpdateAsync<T>")]
        public Task<bool> UpdateAsync()
        {
            Step();
            var entity = InsertData();
            return _connection.UpdateAsync(entity);
        }

        [Benchmark(Description = "Get<T>")]
        public Post Get()
        {
            Step();
            var entity = InsertData();
            return _connection.Get<Post>(entity.Id);
        }


        [Benchmark(Description = "GetAsync<T>")]
        public Task<Post> GetAsync()
        {
            Step();
            var entity = InsertData();
            return _connection.GetAsync<Post>(entity.Id);
        }
        private Post InsertData()
        {
            var entity = CreatePost();
            entity.Id = (int)_connection.Insert(entity);
            return entity;
        }

        private async Task<Post> InsertDataAsync()
        {
            var entity = CreatePost();
            entity.Id = (await _connection.InsertAsync(entity));
            return entity;
        }
    }
}
