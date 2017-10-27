using Orchard.Messaging;
using Orchard.RabbitMQ.Messaging;
using System;

namespace Orchard.RabbitMQPublishConsole
{
    public class Program {
        public static void Main(string[] args) {
            Console.WriteLine("Enter the message and press enter to send:");

            IMessageBus messageBus = new RabbitMQMessageBus(new RabbitMQMessageBusOptions { ConnectionString = "amqp://localhost", Topic = "OrchardQueue", ExchangeName = "OrchardExchange",VirtualHost="test" });
            string message;
            do {
                message = Console.ReadLine();
                messageBus.PublishAsync(message);
            } while (message != null);

            messageBus.Dispose();
        }
    }
}