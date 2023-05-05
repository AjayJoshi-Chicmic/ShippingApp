using ShippingApp.Models;

namespace ShippingApp.Services
{
    public interface IApiCallingService
    {
        public ProductType GetProductType(Guid? productTypeId);
        public ContainerTypeModel GetContainerType(Guid? containerTypeId);
        public double GetDistance(CheckpointModel cp1, CheckpointModel cp2);
    }
}
