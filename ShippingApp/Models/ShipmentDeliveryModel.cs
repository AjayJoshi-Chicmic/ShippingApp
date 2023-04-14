namespace ShippingApp.Models
{
    public class ShipmentDeliveryModel
    {
        public ShipmentModel shipment { get; set; } = new ShipmentModel();
        public List<CheckpointModel>? checkpoints { get; set; } 
    }
}
