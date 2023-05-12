using System.ComponentModel.DataAnnotations;

namespace ShippingApp.Models
{
	public class GetCheckpointModel
	{
		public Guid checkpointId { get; set; } 
		public string checkpointName { get; set; } = string.Empty;
		public float latitude { get; set; }
		public float longitude { get; set; }

		public GetCheckpointModel()
		{

		}
		public GetCheckpointModel(CheckpointModel checkpoint)
		{
			this.checkpointId = checkpoint.checkpointId;
			this.checkpointName = checkpoint.checkpointName;
			this.latitude = checkpoint.latitude;
			this.longitude = checkpoint.longitude;
		}
	}
}
