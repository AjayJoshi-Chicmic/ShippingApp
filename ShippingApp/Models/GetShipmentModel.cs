namespace ShippingApp.Models
{
    public class GetShipmentModel
    {
        public Guid shipmentId { get; set; }
        public Guid customerId { get; set; }
        public Guid productTypeId { get; set; }
        public Guid cointainerTypeId { get; set; }
        public float shipmentPrice { get; set; } = 0;
        public float shipmentWeight { get; set; } = 0;
        public string origin { get; set; } = string.Empty;
        public string destination { get; set; } = string.Empty;
        public DateTime dateOfOrder { get; set; } = DateTime.Now;
        public Guid shipmentStatusId { get; set; }
        public string notes { get; set; } = string.Empty;
        public string shipmentStatus { get; set; } = string.Empty;
        public string currentLocation { get; set; } = string.Empty;
        public DateTime lastUpdated { get; set; } = DateTime.Now;

        public GetShipmentModel()
        {

        }
        public GetShipmentModel(ShipmentModel shipment,ShipmentStatusModel shipmentStatus)
        {
            this.shipmentId = shipment.shipmentId;
            this.customerId = shipment.customerId;
            this.shipmentWeight = shipment.shipmentWeight;
            this.productTypeId = shipment.productTypeId;
            this.cointainerTypeId = shipment.cointainerTypeId;
            this.origin = shipment.origin;
            this.destination = shipment.destination;
            this.notes = shipment.notes;
            this.dateOfOrder= shipment.dateOfOrder;
            this.shipmentStatusId= shipment.shipmentStatusId;
            this.shipmentStatus = shipmentStatus.shipmentStatus;
            this.currentLocation = shipmentStatus.currentLocation;
            this.lastUpdated = shipment.lastUpdated;
        }
    }
}
