namespace Orchard.Caching {
    /// <summary>
    /// 
    /// </summary>
    public interface ICacheContextAccessor {
        /// <summary>
        /// 
        /// </summary>
        IAcquireContext Current { get; set; }
    }
}