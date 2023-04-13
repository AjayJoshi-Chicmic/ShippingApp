namespace ShippingApp.Models
{
    public class getBestRouteCheckpoint
    {
        public float cost { get; set; }
        public CheckpointModel cp1 { get; set; } = new CheckpointModel();
        public CheckpointModel cp2 { get; set; } = new CheckpointModel();
        public getBestRouteCheckpoint()
        {

        }
        public getBestRouteCheckpoint(float cost, CheckpointModel cp1, CheckpointModel cp2)
        {
            this.cost = cost;
            this.cp1 = cp1;
            this.cp2 = cp2;
        }
    }
}
