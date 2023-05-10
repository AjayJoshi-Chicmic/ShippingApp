using ShippingApp.Models;

namespace ShippingApp.Services
{
    public interface IShortestRoute
    {
        public List<CheckpointModel>shortRoute(CheckpointModel cp1, CheckpointModel cp2);
    }
}
