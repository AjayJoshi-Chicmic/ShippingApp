using ShippingApp.Models;

namespace ShippingApp.Services
{
    public interface IMessageProducerService
    {
        public ResponseModel producer(string queueName, object message);
    }
}
