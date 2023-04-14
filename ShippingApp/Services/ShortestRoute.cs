using ShippingApp.Data;
using ShippingApp.Models;

namespace ShippingApp.Services
{
    public class ShortestRoute : IShortestRoute
    {
        private readonly shipmentAppDatabase _db;

        public ShortestRoute(shipmentAppDatabase _db)
        {
            this._db = _db;
        }
        List<CheckpointModel> cps = new List<CheckpointModel>();
        public List<CheckpointModel> bestRoute(CheckpointModel cp1,CheckpointModel cp2)
        {
            var Route = _db.Routes.Where(x => ((x.checkpoint1Id == cp1.checkpointId) && (x.checkpoint2Id == cp2.checkpointId))||((x.checkpoint2Id == cp1.checkpointId) && (x.checkpoint1Id == cp2.checkpointId))).Select(x => x).ToList();
            if (Route.Count > 0)
            {
                if (Route.First().checkpoint1Id== cp1.checkpointId && Route.First().checkpoint2Id == cp2.checkpointId)
                {
                    var rcps = _db.RouteCheckpoints.Where(x => x.routeId == Route.First().routeId).Select(x => x).OrderBy(x=>x.index).ToList();
                    foreach (var rcp in rcps)
                    {
                        var cp = _db.ShipmentCheckpoints.Where(x => x.checkpointId == rcp.checkpointId).First();
                        cps.Add(cp);
                    }
                }
                else
                {
                    var rcps = _db.RouteCheckpoints.Where(x => x.routeId == Route.First().routeId).Select(x => x).OrderByDescending(x => x.index).ToList();
                    foreach (var rcp in rcps)
                    {
                        var cp = _db.ShipmentCheckpoints.Where(x => x.checkpointId == rcp.checkpointId).First();
                        cps.Add(cp);
                    }
                }
                return cps;
            }
            List<CheckpointModel> cpList= new List<CheckpointModel>();
            var checkpoints = _db.ShipmentCheckpoints.Where(x=>(x.checkpointId != cp1.checkpointId) && (x.checkpointId != cp2.checkpointId)).Select(x => x).ToList();
            temp = checkpoints;
            cps.Add(cp1);
            var cost = getCost(cp1,cp2);
            cps.Add(cp2);
            RouteModel route= new RouteModel(cp1,cp2);
            _db.Routes.Add(route);
            int i = 1;
            foreach(var cp in cps)
            {
                RouteCheckpointsModel RouteCp = new RouteCheckpointsModel(route.routeId, cp.checkpointId, i);
                _db.RouteCheckpoints.Add(RouteCp);
                i++;
            }
            _db.SaveChanges();
            return cps;
        }

        List<CheckpointModel> temp = new List<CheckpointModel>();

        public  getBestRouteCheckpoint getCost(CheckpointModel cp1, CheckpointModel cp2)
        {
            var costs = _db.CheckpointMappings.Where(x => (x.checkpoint1Id == cp1.checkpointId && x.checkpoint2Id == cp2.checkpointId) || (x.checkpoint1Id == cp2.checkpointId && x.checkpoint2Id == cp1.checkpointId)).Select(x => x.cost).ToList();
            var temp2 = temp.Where(x => x.checkpointId != cp1.checkpointId && x.checkpointId != cp2.checkpointId).Select(x => x).ToList();
            temp = temp2;
            if (temp.Count == 0) 
            {
                return new getBestRouteCheckpoint(costs.First(),cp1,cp2);
            }
            float cost = costs.First();
            float tempCost = 0;
            var shortRoute = new getBestRouteCheckpoint(0,cp1,cp2);
            foreach (var checkpoint in temp)
            {
                tempCost = getCost(cp1, checkpoint).cost + getCost(cp2, checkpoint).cost;
                if (cost > tempCost)
                {
                    cost = tempCost;
                    shortRoute.cp1 = checkpoint;
                    shortRoute.cp2 = cp2;
                    shortRoute.cost = cost;
                }
            }
            if(shortRoute.cost> 0)
            {
                cps.Add(shortRoute.cp1);
            }
            return shortRoute;
        }
    }
}
