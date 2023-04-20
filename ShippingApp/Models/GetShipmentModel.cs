namespace ShippingApp.Models
{
    public class GetShipmentModel
    {
        public Guid shipmentId { get; set; }
        public Guid customerId { get; set; }
        public string productType { get; set; } = string.Empty;
        public string cointainerType { get; set; } = string.Empty;
        public float shipmentPrice { get; set; } = 0;
        public float shipmentWeight { get; set; } = 0;
        public string origin { get; set; } = string.Empty;
        public string destination { get; set; } = string.Empty; 
        public DateTime dateOfOrder { get; set; } = DateTime.Now;
        public Guid shipmentStatusId { get; set; } = Guid.Empty;
        public string notes { get; set; } = string.Empty;
        public string shipmentStatus { get; set; } = string.Empty;
        public string currentLocation { get; set; } = string.Empty;
        public DateTime lastUpdated { get; set; } = DateTime.Now;

        public GetShipmentModel()
        {

        }
        public GetShipmentModel(ShipmentModel shipment,ShipmentStatusModel shipmentStatus,string location,string originName,string destinationName,string productType,string containerType)
        {
            this.shipmentId = shipment.shipmentId;
            this.customerId = shipment.customerId;
            this.shipmentWeight = shipment.shipmentWeight;
            this.shipmentPrice= shipment.shipmentPrice;
            this.productType = productType;
            this.cointainerType = containerType;
            this.origin = originName;
            this.destination = destinationName;
            this.notes = shipment.notes;
            this.dateOfOrder= shipment.dateOfOrder;
            this.shipmentStatusId= shipment.shipmentStatusId;
            this.shipmentStatus = shipmentStatus.shipmentStatus;
            this.currentLocation = location;
            this.lastUpdated = shipmentStatus.lastUpdated;
        }
    }
}
