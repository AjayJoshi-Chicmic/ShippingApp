using ShippingApp.Models;

namespace ShippingApp.Services
{
    public interface IShortestRoute
    {
        public List<CheckpointModel> bestRoute(CheckpointModel cp1, CheckpointModel cp2);
    }
}
