using ShippingApp.Models;

namespace ShippingApp.Services
{
    public interface IShipmentService
    {
        public ResponseModel AddShipment(AddShipmentModel shipment);
        public ResponseModel ChangeStatus(ShipmentStatusModel status);
        public ResponseModel GetShipment(Guid shipmentId, Guid customerId, Guid productTypeId, Guid containerTypeId);
    }
}
