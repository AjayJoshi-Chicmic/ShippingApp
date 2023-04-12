using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using ShippingApp.Models;
using System.Text.Json;

namespace ShippingApp.Services
{
    public class BackgroundTaskService : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        //private readonly ShipmentService _shipmentService;
        public BackgroundTaskService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(() => Consumer("createShipment"));
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource.Cancel();
            await Task.CompletedTask;
        }
        public void Consumer(string queueName)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var shipmentService = scope.ServiceProvider.GetService<IShipmentService>();
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
                    AddShipmentModel shipment = JsonSerializer.Deserialize<AddShipmentModel>(message)!;
                    if (shipment != null)
                    {
                        var response = shipmentService!.AddShipment(shipment!);
                    }
                };
                channel.BasicConsume(queueName, true, consumer);
                Console.ReadLine();
            }
        }
    }
}
