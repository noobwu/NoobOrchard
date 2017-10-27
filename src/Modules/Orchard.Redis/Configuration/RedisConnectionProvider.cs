using System;
using System.Collections.Concurrent;
using System.Configuration;
using Orchard.Environment.Configuration;
using Orchard.Logging;
using StackExchange.Redis;
using Orchard.UI.Notify;
using Orchard.Localization;

namespace Orchard.Redis.Configuration {

    /// <summary>
    /// 
    /// </summary>
    public class RedisConnectionProvider : IRedisConnectionProvider {
        private static ConcurrentDictionary<string, Lazy<ConnectionMultiplexer>> _connectionMultiplexers = new ConcurrentDictionary<string, Lazy<ConnectionMultiplexer>>();
        private readonly ShellSettings _shellSettings;

        public RedisConnectionProvider(ShellSettings shellSettings) {
            _shellSettings = shellSettings;
            Logger = NullLogger.Instance;
        }

        public Localizer T { get; set; }

        public ILogger Logger { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>

        public string GetConnectionString(string service) {
            var _tenantSettingsKey = _shellSettings.Name + ":" + service;
            var _defaultSettingsKey = service;

            var connectionStringSettings = ConfigurationManager.ConnectionStrings[_tenantSettingsKey] 
                ?? ConfigurationManager.ConnectionStrings[_defaultSettingsKey];

            if (connectionStringSettings == null) {
                return null;
            }

            return connectionStringSettings.ConnectionString;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public ConnectionMultiplexer GetConnection(string connectionString) {

            if (String.IsNullOrWhiteSpace(connectionString)) {
                return null;
            }

            // when using ConcurrentDictionary, multiple threads can create the value
            // at the same time, so we need to pass a Lazy so that it's only 
            // the object which is added that will create a ConnectionMultiplexer,
            // even when a delegate is passed

            return _connectionMultiplexers.GetOrAdd(connectionString,
                new Lazy<ConnectionMultiplexer>(() => {
                    Logger.DebugFormat("Creating a new cache client for: {0}", connectionString);
                    return ConnectionMultiplexer.Connect(connectionString);
                })).Value;
        }
    }
}
