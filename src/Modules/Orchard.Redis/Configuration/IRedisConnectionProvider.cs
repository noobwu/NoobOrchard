using StackExchange.Redis;

namespace Orchard.Redis.Configuration {

    /// <summary>
    /// 
    /// </summary>
    public interface IRedisConnectionProvider : ISingletonDependency {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        ConnectionMultiplexer GetConnection(string connectionString);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        string GetConnectionString(string service);
    }

}