using ShippingApp.Models;

namespace ShippingApp.Services
{
    public interface IShortestRoute
    {
		public List<GetCheckpointModel> shortRoute(CheckpointModel cp1, CheckpointModel cp2);

	}
}
