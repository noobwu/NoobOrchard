using StackExchange.Redis;
using System;
using Orchard.Tests.Utility;
namespace Orchard.Redis.Tests
{
    public static class SharedConnection {
        private static ConnectionMultiplexer _muxer;

        public static ConnectionMultiplexer GetMuxer() {
            string connectionString = Orchard.Tests.Utility.Configuration.GetConnectionString("RedisConnectionString");
            if (String.IsNullOrEmpty(connectionString))
                return null;

            if (_muxer == null) {
                _muxer = ConnectionMultiplexer.Connect(connectionString);
                _muxer.PreserveAsyncOrder = false;
            }

            return _muxer;
        }
    }
}
