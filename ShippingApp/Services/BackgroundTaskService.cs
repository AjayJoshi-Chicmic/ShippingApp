
namespace ShippingApp.Services
{
    public class BackgroundTaskService : IHostedService
    {
        //variables
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        //constructor with DI
        public BackgroundTaskService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        //-----------A function to start tasks ------------------->
        public Task StartAsync(CancellationToken cancellationToken)
        {
            var scope = _scopeFactory.CreateScope();
            //using service
            var messageQueueService = scope.ServiceProvider.GetService<IMessageQueueService>();
            //invoking consumer function to consume from queue
            Task.Run(() => messageQueueService!.Consumer("shipmentStatus"));
            Task.Run(() => messageQueueService!.Consumer("createShipment"));
            Task.Run(() => messageQueueService!.Consumer("sendEmail"));
            Task.Run(() => messageQueueService!.Consumer("shortestRoute"));
            return Task.CompletedTask;   
        }
        //---------------A Function to stop task ----------------> 
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource.Cancel();
            await Task.CompletedTask;
        }
    }
}
