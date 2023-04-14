using System.ComponentModel.DataAnnotations;

namespace ShippingApp.Models
{
    public class ShipmentStatusModel
    {
        [Key]
        public Guid shipmentStatusId { get; set; } = Guid.NewGuid();
        public Guid shipmentId { get; set; }
        public string shipmentStatus { get; set; } = string.Empty;
        public Guid currentLocation { get; set; } = Guid.Empty;
        public DateTime lastUpdated { get; set; } = DateTime.Now;
        public ShipmentStatusModel()
        {

        }
        public ShipmentStatusModel(Guid shipmentId)
        {
            this.shipmentId = shipmentId;
            this.shipmentStatus = "accepted";
        }
    }
}
