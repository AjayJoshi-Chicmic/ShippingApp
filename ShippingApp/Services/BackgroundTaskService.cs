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
            var scope = _scopeFactory.CreateScope();
                var messageQueueService = scope.ServiceProvider.GetService<IMessageQueueService>();
                Task.Run(() => messageQueueService!.Consumer("createShipment"));
                Task.Run(() => messageQueueService!.Consumer("sendEmail"));
                Task.Run(() => messageQueueService!.Consumer("shortestRoute"));
                return Task.CompletedTask;   
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource.Cancel();
            await Task.CompletedTask;
        }
    }
}
