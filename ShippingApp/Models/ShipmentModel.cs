using System.ComponentModel.DataAnnotations;

namespace ShippingApp.Models
{
    public class ShipmentModel
    {
        [Key]
        public Guid shipmentId { get; set; } = Guid.NewGuid();
        public Guid customerId { get; set; }
        public Guid productTypeId { get; set; }
        public Guid cointainerTypeId { get; set; }
        public float shipmentPrice { get; set; } = 0;
        public float shipmentWeight { get;set; } = 0;
        public string origin { get; set; } = string.Empty;
        public string destination { get; set; } = string.Empty;
        public DateTime dateOfOrder { get; set; } = DateTime.Now;
        public Guid shipmentStatusId { get; set; }
        public string notes { get; set; } = string.Empty;
        public bool isDeleted { get; set; } = false;
        public DateTime lastUpdated { get; set; } = DateTime.Now;

        public ShipmentModel()
        {

        }
        public ShipmentModel(AddShipmentModel shipment)
        {
            this.customerId = shipment.customerId;
            this.shipmentWeight= shipment.shipmentWeight;
            this.productTypeId= shipment.productTypeId;
            this.cointainerTypeId = shipment.containerTypeId;
            this.origin = shipment.origin;
            this.destination = shipment.destination;
            this.notes = shipment.notes;
        }
    }
}
