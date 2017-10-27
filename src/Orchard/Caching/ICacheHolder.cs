using System;

namespace Orchard.Caching {
    /// <summary>
    /// 
    /// </summary>
    public interface ICacheHolder : ISingletonDependency {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="component"></param>
        /// <returns></returns>
        ICache<TKey, TResult> GetCache<TKey, TResult>(Type component);
    }
}
