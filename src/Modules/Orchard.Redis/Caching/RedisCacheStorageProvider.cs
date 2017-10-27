using Newtonsoft.Json;
using Orchard.Caching.Services;
using Orchard.Environment.Configuration;
using Orchard.Environment.Extensions;
using Orchard.Logging;
using Orchard.Redis.Configuration;
using Orchard.Redis.Extensions;
using StackExchange.Redis;
using System;

namespace Orchard.Redis.Caching {

    [OrchardFeature("Orchard.Redis.Caching")]
    [OrchardSuppressDependency("Orchard.Caching.Services.DefaultCacheStorageProvider")]
    public class RedisCacheStorageProvider : Component, ICacheStorageProvider {
        public const string ConnectionStringKey = "Orchard.Redis.Cache";

        private readonly ShellSettings _shellSettings;
        private readonly IRedisConnectionProvider _redisConnectionProvider;
        private readonly string _connectionString;
        private readonly ConnectionMultiplexer _connectionMultiplexer;
        public IDatabase Database {
            get {
                return _connectionMultiplexer.GetDatabase();
            }
        }
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="shellSettings"></param>
        ///// <param name="redisConnectionProvider"></param>
        //public RedisCacheStorageProvider(ShellSettings shellSettings, 
        //    IRedisConnectionProvider redisConnectionProvider) {
        //    _shellSettings = shellSettings;
        //    _redisConnectionProvider = redisConnectionProvider;
        //    _connectionString = _redisConnectionProvider.GetConnectionString(ConnectionStringKey);
        //    _connectionMultiplexer = _redisConnectionProvider.GetConnection(_connectionString);

        //    Logger = NullLogger.Instance;
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="shellSettings"></param>
        /// <param name="connectionString"></param>
        /// <param name="redisConnectionProvider"></param>
        public RedisCacheStorageProvider(ShellSettings shellSettings,string connectionString,
            IRedisConnectionProvider redisConnectionProvider)
        {
            _shellSettings = shellSettings;
            _redisConnectionProvider = redisConnectionProvider;
            _connectionString = connectionString;
            _connectionMultiplexer = _redisConnectionProvider.GetConnection(_connectionString);

            Logger = NullLogger.Instance;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Get<T>(string key) {
            var json = Database.StringGet(GetLocalizedKey(key));
            if(String.IsNullOrEmpty(json)) {
                return null;
            }
            return JsonConvert.DeserializeObject<T>(json);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Put<T>(string key, T value) {
            var json = JsonConvert.SerializeObject(value);
            Database.StringSet(GetLocalizedKey(key), json, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="validFor"></param>
        public void Put<T>(string key, T value, TimeSpan validFor) {
            var json = JsonConvert.SerializeObject(value);
            Database.StringSet(GetLocalizedKey(key), json, validFor);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key) {
            Database.KeyDelete(GetLocalizedKey(key));
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear() {
            _connectionMultiplexer.KeyDeleteWithPrefix(GetLocalizedKey("*"));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetLocalizedKey(string key) {
            return _shellSettings.Name + ":Cache:" + key;
        }
    }
}
