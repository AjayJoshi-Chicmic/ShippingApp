namespace ShippingApp.Models
{
    public class ShipmentDeliveryModel
    {
        public SendShipmentModel shipment { get; set; } = new SendShipmentModel();
        public List<CheckpointModel>? checkpoints { get; set; } 
    }
}
