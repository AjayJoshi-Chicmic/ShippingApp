namespace ShippingApp.Models
{
    public class AddShipmentModel
    {
        public Guid customerId { get; set; }
        public Guid productTypeId { get; set; } 
        public Guid containerTypeId { get; set; } 
        public float shipmentWeight { get; set; } = 0;
        public string origin { get; set; } = string.Empty;
        public string destination { get; set; } = string.Empty;
        public string notes { get; set; } = string.Empty;
    }
}
