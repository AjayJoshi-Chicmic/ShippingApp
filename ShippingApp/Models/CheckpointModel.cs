using System.ComponentModel.DataAnnotations;

namespace ShippingApp.Models
{
    public class CheckpointModel
    {
        [Key]
        public Guid checkpointId { get; set; } = Guid.NewGuid();
        public string checkpointName { get; set; } = string.Empty;
        public float latitude { get; set; }
        public float longitude { get; set; }

        public CheckpointModel()
        {

        }
        public CheckpointModel(AddCheckpointModel checkpoint)
        {
            this.checkpointName = checkpoint.checkpointName;
            this.latitude = checkpoint.latitude;
            this.longitude = checkpoint.longitude;
        }
    }
}
