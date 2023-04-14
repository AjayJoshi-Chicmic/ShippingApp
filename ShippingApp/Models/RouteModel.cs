using System.ComponentModel.DataAnnotations;

namespace ShippingApp.Models
{
    public class RouteModel
    {
        [Key]
        public Guid routeId { get; set; } = Guid.NewGuid(); 
        public Guid checkpoint1Id { get; set; }
        public Guid checkpoint2Id { get; set; }
        public RouteModel()
        {

        }
        public RouteModel(CheckpointModel cp1,CheckpointModel cp2)
        {
            this.checkpoint1Id = cp1.checkpointId;
            this.checkpoint2Id = cp2.checkpointId;
        }
    }
}
