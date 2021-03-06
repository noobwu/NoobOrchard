﻿using System;
using RabbitMQ.Client;

namespace Orchard.Messaging.RabbitMq
{
    public class RabbitMqMessageFactory : IMessageFactory
    {
        public ConnectionFactory ConnectionFactory { get; private set; }
        public Action<string, IBasicProperties, IMessage> PublishMessageFilter { get; set; }
        public Action<string, BasicGetResult> GetMessageFilter { get; set; }

        private int retryCount;
        public int RetryCount
        {
            get { return retryCount; }
            set
            {
                if (value < 0 || value > 1)
                    throw new ArgumentOutOfRangeException(nameof(RetryCount), 
                        "Rabbit MQ RetryCount must be 0-1");

                retryCount = value;
            }
        }

        public bool UsePolling { get; set; } 

        public RabbitMqMessageFactory(string connectionString = "localhost",
            string username = null, string password = null,string virtualHost=null)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            ConnectionFactory = new ConnectionFactory {
                RequestedHeartbeat = 10,
            };

            if (!string.IsNullOrEmpty(username))
                ConnectionFactory.UserName = username;
            if (!string.IsNullOrEmpty(password))
                ConnectionFactory.Password = password;
            if (!string.IsNullOrEmpty(virtualHost))
                ConnectionFactory.VirtualHost = virtualHost;
            if (connectionString.StartsWith("amqp://"))
            {
                ConnectionFactory.Uri =new Uri(connectionString);
            }
            else
            {
                var parts = connectionString.SplitOnFirst(':');
                var hostName = parts[0];
                ConnectionFactory.HostName = hostName;
                if (parts.Length > 1)
                {
                    ConnectionFactory.Port = parts[1].ToInt();
                }
            }
        }

        public RabbitMqMessageFactory(ConnectionFactory connectionFactory)
        {
            ConnectionFactory = connectionFactory;
        }

        public virtual IMessageQueueClient CreateMessageQueueClient()
        {
            return new RabbitMqQueueClient(this) {
                RetryCount = RetryCount,
                PublishMessageFilter = PublishMessageFilter,
                GetMessageFilter = GetMessageFilter,
            };
        }

        public virtual IMessageProducer CreateMessageProducer()
        {
            return new RabbitMqProducer(this) {
                RetryCount = RetryCount,
                PublishMessageFilter = PublishMessageFilter,
                GetMessageFilter = GetMessageFilter,
            };
        }

        public virtual void Dispose()
        {
        }
    }
}