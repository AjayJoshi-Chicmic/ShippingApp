namespace ShippingApp.Models
{
    public class AddShipmentModel
    {
        public Guid customerId { get; set; }
        public string productName { get; set; } = string.Empty;
        public string type { get; set; } = string.Empty;
        public int quantity { get; set; }
        public decimal? shipmentWeight { get; set; }
        public string origin { get; set; } = string.Empty;
        public string destination { get; set; } = string.Empty;
        public string notes { get; set; } = string.Empty;
    }
}
