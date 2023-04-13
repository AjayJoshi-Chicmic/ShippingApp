using System.ComponentModel.DataAnnotations;

namespace ShippingApp.Models
{
    public class CheckpointsDistanceModel
    {
        [Key]
        public Guid distancMapId { get; set; } = Guid.NewGuid();
        public Guid checkpoint1Id { get; set; }
        public Guid checkpoint2Id { get; set; }
        public float distance { get; set; } = 0;
        public float cost { get; set; } = 0;    
    }
}
