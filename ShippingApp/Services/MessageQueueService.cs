using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ShippingApp.Models;
using System.Text;
using System.Text.Json;

namespace ShippingApp.Services
{ 
    /// <summary>
    /// 
    /// A message queue service 
    /// It has a consumer function which consumes all the queues
    /// 
    /// </summary>

    public class MessageQueueService : IMessageQueueService
    {
		private readonly IConfiguration configuration;

		//variables
		private readonly IShipmentService shipmentService;
        private readonly IEmailService emailService;

        //constructors with dependency injection
        public MessageQueueService(IShipmentService shipmentService,IEmailService emailService,IConfiguration configuration)
        {
            this.configuration = configuration;
            this.shipmentService = shipmentService;
            this.emailService = emailService;
        }

        //------------A Function to consume Data from queue -------->>
        public void Consumer(string queueName)
        {
            //connecting with rabbit mq 
            var factory = new ConnectionFactory
            {
                Uri
                = new Uri(configuration.GetSection("RabbitMQ:Url").Value!)
            };
            // Adding queue configuration
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
                //response
                var body = e.Body.ToArray();
                //converting body to string
                var message = Encoding.UTF8.GetString(body);

                //for createShipment queue
                if (queueName == "createShipment")
                {
                    //deserializing data
                    AddShipmentModel shipment = JsonSerializer.Deserialize<AddShipmentModel>(message)!;
                    if (shipment != null)
                    {
                        //calling service
                        var response = shipmentService!.AddShipment(shipment!);
                    }
                }
                //for sendemail queue
                else if (queueName == "sendEmail")
                {
                    //deserializing data
                    EmailModel email = JsonSerializer.Deserialize<EmailModel>(message)!;
                    if (email != null)
                    {
                        //calling service
                        var response = emailService!.SendEmail(email);
                    }
                }
                //for shipment status
                else if (queueName == "shipmentStatus")
                {
                    //deserializing data
                    ShipmentStatusModel status = JsonSerializer.Deserialize<ShipmentStatusModel>(message)!;
                    if (status != null)
                    {
                        //calling service
                        var response = shipmentService!.ChangeStatus(status);
                    }
                }
            };
            channel.BasicConsume(queueName, true, consumer);
            Console.ReadLine();

        }
    }
}
