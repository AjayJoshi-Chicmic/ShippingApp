namespace ShippingApp.Models
{
    public class RouteCheckpointsModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid routeId { get; set; }
        public Guid checkpointId { get; set; }
        public int index { get; set; }
        public RouteCheckpointsModel()
        {

        }
        public RouteCheckpointsModel(Guid routeId,Guid checkpointId, int index)
        {
            this.routeId = routeId;
            this.checkpointId = checkpointId;
            this.index = index;
        }
    }
}
