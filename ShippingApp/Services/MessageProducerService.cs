using RabbitMQ.Client;
using ShippingApp.Models;
using System.Text.Json;
using System.Text;

namespace ShippingApp.Services
{
    public class MessageProducerService : IMessageProducerService
    {
        //----------- A Function to produce message in queue ------------>
        public ResponseModel producer(string queueName, object message)
        {
            //connecting with rabbit mq
            var factory = new ConnectionFactory
            {
                Uri
                = new Uri("amqp://s2:guest@192.180.3.63:5672")
            };
            //Configuring Queue
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.QueueDeclare(queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            //data to send in body
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            channel.BasicPublish("", queueName,null, body);
            return new ResponseModel();
        }
    }
}
