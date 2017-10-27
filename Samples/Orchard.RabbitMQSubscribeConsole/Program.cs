using Orchard.Messaging;
using Orchard.RabbitMQ.Messaging;
using System;
namespace Orchard.RabbitMQSubscribeConsole {
    public class Program {
        public static void Main(string[] args) {
            Console.WriteLine("Waiting to receive messages....");

            IMessageBus messageBus = new RabbitMQMessageBus(new RabbitMQMessageBusOptions { ConnectionString = "amqp://localhost", Topic = "OrchardQueue", ExchangeName = "OrchardExchange",VirtualHost="test" });
            messageBus.SubscribeAsync<string>(msg => { Console.WriteLine(msg); }).GetAwaiter().GetResult();
            Console.ReadLine();
        }
    }
}