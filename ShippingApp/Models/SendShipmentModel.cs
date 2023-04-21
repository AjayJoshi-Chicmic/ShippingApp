namespace ShippingApp.Models
{
    public class SendShipmentModel
    {
        public Guid shipmentId { get; set; }
        public string productType { get; set; } = string.Empty;
        public string containerType { get; set; } = string.Empty;
        public float shipmentWeight { get; set; }
        public Guid origin { get; set; }
        public Guid Destination { get; set; }
    }
}
