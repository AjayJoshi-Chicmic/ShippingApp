namespace ShippingApp.Models
{
    public class AddShipmentModel
    {
        public Guid customerId { get; set; }
        public Guid productTypeId { get; set; } 
        public Guid containerTypeId { get; set; } 
        public float shipmentWeight { get; set; } = 0;
        public Guid origin { get; set; } = Guid.Empty;
        public Guid destination { get; set; } = Guid.Empty;
        public string notes { get; set; } = string.Empty;
        public Guid transactionRecordId { get; set; } = Guid.Empty;
    }
}
