using ShippingApp.Models;

namespace ShippingApp.Services
{
    public interface IShortestRoute
    {
        //public List<CheckpointModel> bestRoute(CheckpointModel cp1, CheckpointModel cp2);
        public List<CheckpointModel> shortRoute(CheckpointModel cp1, CheckpointModel cp2);
    }
}
