using System;

// ReSharper disable once CheckNamespace
namespace Orchard.Caching.Services {
    /// <summary>
    /// 
    /// </summary>
    public static class CachingExtensions {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(this ICacheStorageProvider provider, string key) {
            return (T)provider.Get<T>(key);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheService"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object Get(this ICacheService cacheService, string key) {
            return cacheService.Get<object>(key);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheService"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(this ICacheService cacheService, string key) {
            return Get<T>(cacheService, key, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheService"></param>
        /// <param name="key"></param>
        /// <param name="validFor"></param>
        /// <returns></returns>
        public static T Get<T>(this ICacheService cacheService, string key, TimeSpan validFor) {
            return Get<T>(cacheService, key, null, validFor);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheService"></param>
        /// <param name="key"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static T Get<T>(this ICacheService cacheService, string key, Func<T> factory) {
            return Get(cacheService, key, factory, TimeSpan.MinValue);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheService"></param>
        /// <param name="key"></param>
        /// <param name="factory"></param>
        /// <param name="validFor"></param>
        /// <returns></returns>
        public static T Get<T>(this ICacheService cacheService, string key, Func<T> factory, TimeSpan validFor) {
            var result = cacheService.GetObject<T>(key);

            if (result == null && factory != null) {
                var computed = factory();

                if (validFor == TimeSpan.MinValue)
                    cacheService.Put(key, computed);
                else
                    cacheService.Put(key, computed, validFor);
                return computed;
            }

            return (T)result;
        }
    }
}