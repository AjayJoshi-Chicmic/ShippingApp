namespace ShippingApp.Models
{
    public class AddCheckpointModel
    {
        public string checkpointName { get; set; } = string.Empty;
        public float latitude { get; set; }
        public float longitude { get; set; }
        public Guid parentCheckpointId { get; set; }
    }
}
