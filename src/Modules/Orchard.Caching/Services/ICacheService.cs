using System;

namespace Orchard.Caching.Services {
    /// <summary>
    /// 
    /// </summary>
    public interface ICacheService : IDependency {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        object GetObject<T>(string key);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Put<T>(string key, T value);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="validFor"></param>
        void Put<T>(string key, T value, TimeSpan validFor);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        void Remove(string key);
        /// <summary>
        /// 
        /// </summary>
        void Clear();

      
    }
}