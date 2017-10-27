using System;

namespace Orchard.Messaging.RabbitMq.Tests
{
    public class Config
    {
        public const string ServiceStackBaseUri = "http://localhost:20000";
        public const string AbsoluteBaseUri = ServiceStackBaseUri + "/";
        public const string ListeningOn = ServiceStackBaseUri + "/";
        public static readonly string RabbitMQVirtualHost = "OrchardTest";//
        public static readonly string RabbitMQConnString =System.Environment.GetEnvironmentVariable("CI_RABBITMQ") ?? "localhost";
        public static readonly string SqlServerBuildDb =  System.Environment.GetEnvironmentVariable("CI_SQLSERVER")
                                            ?? "Server=localhost;Database=test;User Id=sa;Password=123456;";
    }
}