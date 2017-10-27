using BenchmarkDotNet.Attributes;
using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;
using Newtonsoft.Json;
using Orchard.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Data.Storage.BenchmarkTests
{
    /// <summary>
    /// 
    /// </summary>
    public class MemcachedBenchmarks : BenchmarkBase
    {
        public const string ConnectionStringKey = "MemcachedBenchmarks";
        private MemcachedClient _client;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        private void LoadClient()
        {
            //Enyim.Caching.LogManager.AssignFactory(null);
            IMemcachedClientConfiguration config = GetClientConfiguration();
            _client = new MemcachedClient(config);
        }
        [GlobalSetup]
        public void Setup()
        {
            LoadClient();
        }
        private IMemcachedClientConfiguration GetClientConfiguration()
        {
            string[] hosts = System.Configuration.ConfigurationManager.AppSettings["MemcachedHost"].Split(',');
            const int defaultPort = 11211;
            const int ipAddressIndex = 0;
            const int portIndex = 1;

            var ipEndpoints = new List<IPEndPoint>();
            foreach (var host in hosts)
            {
                var hostParts = host.Split(':');
                if (hostParts.Length == 0)
                    throw new ArgumentException("'{0}' is not a valid host IP Address: e.g. '127.0.0.0[:11211]'");

                var port = (hostParts.Length == 1) ? defaultPort : int.Parse(hostParts[portIndex]);

                var hostAddresses = Dns.GetHostAddresses(hostParts[ipAddressIndex]);
                foreach (var ipAddress in hostAddresses)
                {
                    var endpoint = new IPEndPoint(ipAddress, port);
                    ipEndpoints.Add(endpoint);
                }
            }
            return PrepareMemcachedClientConfiguration(ipEndpoints);
        }
        /// <summary>
        /// Prepares a MemcachedClientConfiguration based on the provided ipEndpoints.
        /// </summary>
        /// <param name="ipEndpoints">The ip endpoints.</param>
        /// <returns></returns>
        private IMemcachedClientConfiguration PrepareMemcachedClientConfiguration(IEnumerable<IPEndPoint> ipEndpoints)
        {
            var config = new MemcachedClientConfiguration();
            foreach (var ipEndpoint in ipEndpoints)
            {
                config.Servers.Add(ipEndpoint);
            }

            config.SocketPool.MinPoolSize = 10;
            config.SocketPool.MaxPoolSize = 100;
            config.SocketPool.ConnectionTimeout = new TimeSpan(0, 0, 10);
            config.SocketPool.DeadTimeout = new TimeSpan(0, 2, 0);

            return config;
        }
        [GlobalCleanup]
        public void Cleanup()
        {
            _client?.Dispose();
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
            var key = GetLocalizedKey(entity.Id.ToString());
            return _client.Remove(key);
        }

        [Benchmark(Description = "DeleteAsync<T>")]
        public Task<bool> DeleteAsync()
        {
            Step();
            var entity = InsertData();
            var key = GetLocalizedKey(entity.Id.ToString());
            return Task.Factory.StartNew(() =>
            {
                return _client.Remove(key);
            });

        }

        [Benchmark(Description = "Update<T>")]
        public bool Update()
        {
            Step();
            var entity = InsertData();
            var key = GetLocalizedKey(entity.Id.ToString());
            return Execute(() => _client.Store(StoreMode.Replace, key, new MemcachedValueWrapper(entity)));
        }

        [Benchmark(Description = "UpdateAsync<T>")]
        public Task<bool> UpdateAsync()
        {
            Step();
            var entity = InsertData();
            var key = GetLocalizedKey(entity.Id.ToString());
            return Task.Factory.StartNew(() =>
            {
                return Execute(() => _client.Store(StoreMode.Replace, key, new MemcachedValueWrapper(entity)));
            });

        }

        [Benchmark(Description = "Get<T>")]
        public Post Get()
        {
            Step();
            var entity = InsertData();
            return Execute(() =>
            {
                var key = GetLocalizedKey(entity.Id.ToString());
                var result = _client.Get<MemcachedValueWrapper>(key);
                if (result != null)
                {
                    return (Post)result.Value;
                }
                return default(Post);
            });
        }


        [Benchmark(Description = "GetAsync<T>")]
        public async Task<Post> GetAsync()
        {
            Step();
            var entity = InsertData();
            return await Task.Factory.StartNew(() =>
            {
                return Execute(() =>
                {
                    var key = GetLocalizedKey(entity.Id.ToString());
                    var result = _client.Get<MemcachedValueWrapper>(key);
                    if (result != null)
                    {
                        return (Post)result.Value;
                    }
                    return default(Post);
                });
            });

        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Post InsertData()
        {
            var entity = CreatePost();
            entity.Id = RandomData.GetInt();
            var key = GetLocalizedKey(entity.Id.ToString());
            Execute(() => _client.Store(StoreMode.Add, key, new MemcachedValueWrapper(entity)));
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
            var key = GetLocalizedKey(entity.Id.ToString());
            await Task.Factory.StartNew(() =>
            {
                Execute(() => _client.Store(StoreMode.Add, key, new MemcachedValueWrapper(entity)));
            });
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

        /// <summary>
        /// Executes the specified expression. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        private T Execute<T>(Func<T> action)
        {
            DateTime before = DateTime.Now;
            Console.Write("Executing action '{0}'", action.Method.Name);

            try
            {
                T result = action();
                TimeSpan timeTaken = DateTime.Now - before;
                Console.Write("Action '{0}' executed. Took {1} ms.", action.Method.Name, timeTaken.TotalMilliseconds);
                return result;
            }
            catch (Exception ex)
            {
                Console.Write("There was an error executing Action '{0}'. Message: {1}", action.Method.Name, ex.Message);
                throw;
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class MemcachedValueWrapper
    {
        public Type ValueType { get; set; }

        [NonSerialized] private object _value;

        /// <summary>
        /// 
        /// </summary>
        public object Value
        {
            get
            {
                return _value;
            }
        }

        public MemcachedValueWrapper() { }

        public MemcachedValueWrapper(object value)
        {
            if (value == null) return;
            ValueType = value.GetType();
            _value = value;
        }
    }
}
