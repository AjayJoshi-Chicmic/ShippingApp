namespace ShippingApp.Models
{
    public class ShipmentDeliveryModel
    {
        public SendShipmentModel shipment { get; set; } = new SendShipmentModel();
        public List<GetCheckpointModel>? checkpoints { get; set; } 
    }
}
