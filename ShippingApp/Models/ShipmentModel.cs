namespace ShippingApp.Models
{
    public class ShipmentModel
    {
        public Guid shipmentId { get; set; } = Guid.NewGuid();
        public Guid customerId { get; set; }
        public int quantity { get; set; }
        public decimal? shipmentPrice { get; set; }
        public decimal? shipmentWeight { get;set; }
        public string origin { get; set; } = string.Empty;
        public string destination { get; set; } = string.Empty;
        public DateTime dateOfOrder { get; set; } = DateTime.Now;
        public Guid shipmentStatusId { get; set; }
        public string notes { get; set; } = string.Empty;
    }
}
