using System;
using System.Collections.Concurrent;

namespace Orchard.Caching {
    /// <summary>
    /// 
    /// </summary>
    public interface ICacheManager {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="key"></param>
        /// <param name="acquire"></param>
        /// <returns></returns>
        TResult Get<TKey, TResult>(TKey key, Func<AcquireContext<TKey>, TResult> acquire);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        ICache<TKey, TResult> GetCache<TKey, TResult>();
    }
    /// <summary>
    /// 
    /// </summary>
    public static class CacheManagerExtensions {
        /// <summary>
        /// 
        /// </summary>
        private static readonly ConcurrentDictionary<object, object> _locks = new ConcurrentDictionary<object, object>();
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="cacheManager"></param>
        /// <param name="key"></param>
        /// <param name="preventConcurrentCalls"></param>
        /// <param name="acquire"></param>
        /// <returns></returns>
        public static TResult Get<TKey, TResult>(this ICacheManager cacheManager, TKey key, bool preventConcurrentCalls, Func<AcquireContext<TKey>, TResult> acquire) {
            if (preventConcurrentCalls) {
                var lockKey = _locks.GetOrAdd(key, _ => new object());
                lock (lockKey) {
                    return cacheManager.Get(key, acquire);
                }
            }
            else {
                return cacheManager.Get(key, acquire);
            }
        }
    }
}
