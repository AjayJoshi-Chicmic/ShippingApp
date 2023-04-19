namespace ShippingApp.Models
{
    public class GetShipmentStatus
    {
        public Guid shipmentId { get; set; }
        public string shipmentStatus { get; set; } = string.Empty;
        public string currentLocation { get; set; } = string.Empty;
        public DateTime lastUpdated { get; set; } = DateTime.Now;
        public GetShipmentStatus()
        {

        }
        public GetShipmentStatus(ShipmentStatusModel shipmentStatus,string currentLocation)
        {
            this.shipmentStatus = shipmentStatus.shipmentStatus;
            this.shipmentId = shipmentStatus.shipmentId;
            this.lastUpdated = shipmentStatus.lastUpdated;
            this.currentLocation = currentLocation;
        }
    }
}
