using BenchmarkDotNet.Attributes;
using Newtonsoft.Json;
using Orchard.Utility;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Data.Storage.BenchmarkTests
{
    /// <summary>
    /// 
    /// </summary>
    public class RedisBenchmarks : BenchmarkBase
    {
        public const string ConnectionStringKey = "RedisBenchmarks";
        private readonly string connectionString="127.0.0.1";
        private ConnectionMultiplexer _connectionMultiplexer;
        public IDatabase Database
        {
            get
            {
                return _connectionMultiplexer.GetDatabase();
            }
        }
        protected override void BaseSetup()
        {
            _connectionMultiplexer = ConnectionMultiplexer.Connect(connectionString);
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
            _connectionMultiplexer?.Dispose();
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
            return Database.KeyDelete(GetLocalizedKey(entity.Id.ToString()));
        }

        [Benchmark(Description = "DeleteAsync<T>")]
        public Task<bool> DeleteAsync()
        {
            Step();
            var entity = InsertData();
            return Database.KeyDeleteAsync(GetLocalizedKey(entity.Id.ToString()));
        }

        [Benchmark(Description = "Update<T>")]
        public bool Update()
        {
            Step();
            var entity = InsertData();
            var json = JsonConvert.SerializeObject(entity);
            return Database.StringSet(GetLocalizedKey(entity.Id.ToString()), json, null);
        }

        [Benchmark(Description = "UpdateAsync<T>")]
        public Task<bool> UpdateAsync()
        {
            Step();
            var entity = InsertData();
            var json = JsonConvert.SerializeObject(entity);
            return Database.StringSetAsync(GetLocalizedKey(entity.Id.ToString()), json);
        }

        [Benchmark(Description = "Get<T>")]
        public Post Get()
        {
            Step();
            var entity = InsertData();
            var json = Database.StringGet(GetLocalizedKey(entity.Id.ToString()));
            if (String.IsNullOrEmpty(json))
            {
                return null;
            }
            return JsonConvert.DeserializeObject<Post>(json);
        }


        [Benchmark(Description = "GetAsync<T>")]
        public async Task<Post> GetAsync()
        {
            Step();
            var entity = InsertData();
            var json =await Database.StringGetAsync(GetLocalizedKey(entity.Id.ToString()));
            if (String.IsNullOrEmpty(json))
            {
                return null;
            }
            return JsonConvert.DeserializeObject<Post>(json);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Post InsertData()
        {
            var entity = CreatePost();
            entity.Id = RandomData.GetInt();
            var json = JsonConvert.SerializeObject(entity);
            Database.StringSet(GetLocalizedKey(entity.Id.ToString()), json, null);
            return entity;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task<Post> InsertDataAsync()
        {
            var entity = CreatePost();
            entity.Id = RandomData.GetInt();
            var json = JsonConvert.SerializeObject(entity);
            await Database.SetAddAsync(GetLocalizedKey(entity.Id.ToString()), json);
            return entity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetLocalizedKey(string key)
        {
            return ConnectionStringKey + ":Cache:" + key;
        }
    }
}
