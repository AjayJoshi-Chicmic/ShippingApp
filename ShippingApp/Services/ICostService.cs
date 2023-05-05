using ShippingApp.Models;

namespace ShippingApp.Services
{
    public interface ICostService
    {
        public ResponseModel shipmentCost(Guid cp1Id, Guid cp2Id, Guid productTypeId, Guid containerTypeId, float weight);
    }
}
