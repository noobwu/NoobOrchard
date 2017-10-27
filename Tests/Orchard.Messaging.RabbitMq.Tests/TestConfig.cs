// Copyright (c) ServiceStack, Inc. All Rights Reserved.
// License: https://raw.github.com/ServiceStack/ServiceStack/master/license.txt


using Orchard.Logging;

namespace Orchard.Messaging.RabbitMq.Tests
{
    public static class TestConfig
    {
        static TestConfig()
        {
            LoggerManager.LogFactory = new ConsoleLoggerFactory();
        }

        public const bool IgnoreLongTests = true;

        public const string SingleHost = "localhost";
        public static readonly string[] MasterHosts = new[] { "localhost" };
        public static readonly string[] SlaveHosts = new[] { "localhost" };
    }
}