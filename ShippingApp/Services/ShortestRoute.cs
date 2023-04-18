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
        int count = 0;
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
                List<CheckpointModel> temp = new List<CheckpointModel>(cps);
                cps.Clear();
                return temp;
            }
            List<CheckpointModel> cpList= new List<CheckpointModel>();
            var checkpoints = _db.ShipmentCheckpoints.Where(x=>(x.checkpointId != cp1.checkpointId) && (x.checkpointId != cp2.checkpointId)).Select(x => x).ToList();
            temp = checkpoints;
            count = checkpoints.Count-1;
            cps.Add(cp1);
            var cost = getCost(cp1,cp2);
            Console.WriteLine(test2.Count());
            foreach(var tests in test2)
            {
                Console.WriteLine(tests.Key);
                Console.WriteLine(tests.Value.Count());
            }
            var minKey = test2.Keys.Min();
            Console.WriteLine(minKey);
            var cpL = test2.Where(x => x.Key == minKey).Select(x => x.Value).ToList().First();
            foreach (var cp in cpL)
            {
                cps.Add(cp);
            }
            //cps.Add(cp2);
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
            
            List<CheckpointModel> tempo = new List<CheckpointModel>(cps);
            cps.Clear();
            return tempo;
        }

        List<CheckpointModel> temp = new List<CheckpointModel>();
        List<CheckpointModel> tempList = new List<CheckpointModel>();
        Dictionary<float,List<CheckpointModel>> test2 = new Dictionary<float,List<CheckpointModel>>();
        
        public  getBestRouteCheckpoint getCost(CheckpointModel cp1, CheckpointModel cp2)
        {
            var costs = _db.CheckpointMappings.Where(x => (x.checkpoint1Id == cp1.checkpointId && x.checkpoint2Id == cp2.checkpointId) || (x.checkpoint1Id == cp2.checkpointId && x.checkpoint2Id == cp1.checkpointId)).Select(x => x.cost).ToList();
            var temp2 = temp.Where(x => x.checkpointId != cp1.checkpointId && x.checkpointId != cp2.checkpointId).Select(x => x).ToList();
            temp = temp2;
            if (temp.Count == 0) 
            { 
                return new getBestRouteCheckpoint(costs.First(),cp1,cp2);
            }
            count--;
            float cost = costs.First();
            Console.WriteLine(costs.First());
            float tempCost = 0;
            var shortRoute = new getBestRouteCheckpoint(0,cp1,cp2);
            var tempPrevCost = cost;
            foreach (var checkpoint in temp)
            {
                Console.WriteLine($"{cp1.checkpointName} " + " " + $"{checkpoint.checkpointName}" + " " + $"{cp2.checkpointName}");
                var c1 = getCost(cp1, checkpoint).cost;
                var c2 = getCost(checkpoint, cp2).cost;
                
                Console.WriteLine(c1 + " " + "cost" + " " + c2 + " " + cost);
                Console.WriteLine(temp.Count() + " " + count);
                tempPrevCost = cost;
                tempCost = c1 + c2;
                if (cost > tempCost)
                {
                    cost = tempCost;
                    shortRoute.cp1 = checkpoint;
                    shortRoute.cp2 = cp2;
                    shortRoute.cost = tempCost;
                }
            }
            
            Console.WriteLine(shortRoute.cost);
            Console.WriteLine(temp.Count());
            shortRoute.cost = cost;
            if (shortRoute.cost > 0)
            {
                Console.WriteLine(shortRoute.cp2.checkpointName);
                tempList.Add(shortRoute.cp2);
            }
            Console.WriteLine(tempList.Count());
            if(temp.Count() == count )
            {
                Console.WriteLine(temp.Count() + " "+ count);
                test2.Add(tempPrevCost, tempList);
                Console.WriteLine(tempList.Count());
            }
            count++;
            return shortRoute;
        }
    }
}
