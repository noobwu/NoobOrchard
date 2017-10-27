using System;
using Orchard.Environment.Configuration;

namespace Orchard.Caching.Services {
    /// <summary>
    /// Provides a per tenant <see cref="ICacheService"/> implementation.
    /// </summary>
    public class DefaultCacheService : ICacheService {
        private readonly ICacheStorageProvider _cacheStorageProvider;
        private readonly string _prefix;

        public DefaultCacheService(
            ShellSettings shellSettings, 
            ICacheStorageProvider cacheStorageProvider) {
            _cacheStorageProvider = cacheStorageProvider;
            _prefix = shellSettings.Name;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetObject<T>(string key) {
            return _cacheStorageProvider.Get<T>(BuildFullKey(key));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Put<T>(string key, T value) {
            _cacheStorageProvider.Put(BuildFullKey(key), value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="validFor"></param>
        public void Put<T>(string key, T value, TimeSpan validFor) {
            _cacheStorageProvider.Put(BuildFullKey(key), value, validFor);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key) {
            _cacheStorageProvider.Remove(BuildFullKey(key));
        }
        /// <summary>
        /// 
        /// </summary>
        public void Clear() {
            _cacheStorageProvider.Clear();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string BuildFullKey(string key) {
            return String.Concat(_prefix, ":", key);
        }
    }
}