using RabbitMQ.Client;
using ShippingApp.Models;
using System.Text.Json;
using System.Text;

namespace ShippingApp.Services
{
    public class MessageProducerService : IMessageProducerService
    {
        public ResponseModel producer(string queueName, object message)
        {
            var factory = new ConnectionFactory
            {
                Uri
                = new Uri("amqp://s2:guest@192.180.3.63:5672")
            };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.QueueDeclare(queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            channel.BasicPublish("", queueName,null, body);
            return new ResponseModel();
        }
    }
}
