using ShippingApp.Models;

namespace ShippingApp.Services
{
    public interface IMessageQueueService
    {
        public ResponseModel producer(string queueName, object message);
        public void Consumer(string queueName);
    }
}
