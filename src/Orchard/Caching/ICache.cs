using System;

namespace Orchard.Caching {
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public interface ICache<TKey, TResult> {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="acquire"></param>
        /// <returns></returns>
        TResult Get(TKey key, Func<AcquireContext<TKey>, TResult> acquire);
    }
}
