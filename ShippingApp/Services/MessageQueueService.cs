using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ShippingApp.Models;
using System.Text;
using System.Text.Json;

namespace ShippingApp.Services
{
    public class MessageQueueService : IMessageQueueService
    {
        public ResponseModel producer(string queueName,object message)
        {
            var factory = new ConnectionFactory
            {
                Uri
                = new Uri("amqp://guest:guest@localhost:5672")
            };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.QueueDeclare(queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            channel.BasicPublish("", queueName, null, body);
            return new ResponseModel();
        }
        public void Consumer(string queueName)
        {
            var factory = new ConnectionFactory
            {
                Uri
                = new Uri("amqp://guest:guest@localhost:5672")
            };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.QueueDeclare(queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(message);
            };
            channel.BasicConsume(queueName, true, consumer);
            Console.ReadLine();
        }
    }
}
