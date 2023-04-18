using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ShippingApp.Models;
using System.Text;
using System.Text.Json;

namespace ShippingApp.Services
{
    public class MessageQueueService : IMessageQueueService
    {
        private readonly IShipmentService shipmentService;
        private readonly IEmailService emailService;

        public MessageQueueService(IShipmentService shipmentService,IEmailService emailService)
        {
            this.shipmentService = shipmentService;
            this.emailService = emailService;
        }
        public void Consumer(string queueName)
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
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                if (queueName == "createShipment")
                {
                    AddShipmentModel shipment = JsonSerializer.Deserialize<AddShipmentModel>(message)!;
                    Console.WriteLine(98);
                    if (shipment != null)
                    {
                        Console.WriteLine(99);
                        var response = shipmentService!.AddShipment(shipment!);
                    }
                }
                else if (queueName == "sendEmail")
                {
                    EmailModel email = JsonSerializer.Deserialize<EmailModel>(message)!;
                    if (email != null)
                    {
                        var response = emailService!.SendEmail(email);
                    }
                }
                else if (queueName == "shipmentStatus")
                {
                    ShipmentStatusModel status = JsonSerializer.Deserialize<ShipmentStatusModel>(message)!;
                    if (status != null)
                    {
                        var response = shipmentService!.ChangeStatus(status);
                    }
                }
            };
            channel.BasicConsume(queueName, true, consumer);
            Console.ReadLine();

        }
    }
}
