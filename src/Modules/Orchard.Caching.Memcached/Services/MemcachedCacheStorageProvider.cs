using System;
using Orchard.Caching.Services;

namespace Orchard.Caching.Memcached.Services {
    public class MemcachedCacheStorageProvider : ICacheStorageProvider {
        private readonly MemcachedClientHolder _clientHolder;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientHolder"></param>
        public MemcachedCacheStorageProvider(MemcachedClientHolder clientHolder) {
            _clientHolder = clientHolder;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Get<T>(string key) {
            return _clientHolder.Get(key);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Put<T>(string key, T value) {
            _clientHolder.Put(key, value);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="validFor"></param>
        public void Put<T>(string key, T value, TimeSpan validFor) {
            _clientHolder.Put(key, value, validFor);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key) {
            _clientHolder.Remove(key);
        }
        /// <summary>
        /// 
        /// </summary>
        public void Clear() {
            _clientHolder.Clear();
        }
    }
}