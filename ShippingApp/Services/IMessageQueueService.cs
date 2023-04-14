using ShippingApp.Models;

namespace ShippingApp.Services
{
    public interface IMessageQueueService
    {
        public void Consumer(string queueName);
    }
}
