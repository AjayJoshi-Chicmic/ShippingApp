using ShippingApp.Data;
using ShippingApp.Models;
using System.Collections.Generic;

namespace ShippingApp.Services
{
    public class ShortestRoute : IShortestRoute
    {
        private readonly shipmentAppDatabase _db;

        public ShortestRoute(shipmentAppDatabase _db)
        {
            this._db = _db;
        }
        
        public float DisCost(CheckpointModel cp1, CheckpointModel cp2)
        {
            var costs = _db.CheckpointMappings.Where(x => (x.checkpoint1Id == cp1.checkpointId && x.checkpoint2Id == cp2.checkpointId) || (x.checkpoint1Id == cp2.checkpointId && x.checkpoint2Id == cp1.checkpointId)).Select(x => x.cost).ToList();
            
            return costs.First();
        }
        public List<CheckpointModel> shortRoute(CheckpointModel cp1, CheckpointModel cp2)
        {
            List<CheckpointModel> cpL = new List<CheckpointModel>();
            if(cp1.checkpointId == cp2.checkpointId)
            {
                cpL.Add(cp1);
                cpL.Add(cp2);
                return cpL;
            }
            var Route = _db.Routes.Where(x => ((x.checkpoint1Id == cp1.checkpointId) && (x.checkpoint2Id == cp2.checkpointId)) || ((x.checkpoint2Id == cp1.checkpointId) && (x.checkpoint1Id == cp2.checkpointId))).Select(x => x).ToList();
            if (Route.Count > 0)
            {
                if (Route.First().checkpoint1Id == cp1.checkpointId && Route.First().checkpoint2Id == cp2.checkpointId)
                {
                    var rcps = _db.RouteCheckpoints.Where(x => x.routeId == Route.First().routeId).Select(x => x).OrderBy(x => x.index).ToList();
                    foreach (var rcp in rcps)
                    {
                        var cp = _db.ShipmentCheckpoints.Where(x => x.checkpointId == rcp.checkpointId).First();
                        cpL.Add(cp);
                    }
                }
                else
                {
                    var rcps = _db.RouteCheckpoints.Where(x => x.routeId == Route.First().routeId).Select(x => x).OrderByDescending(x => x.index).ToList();
                    foreach (var rcp in rcps)
                    {
                        var cp = _db.ShipmentCheckpoints.Where(x => x.checkpointId == rcp.checkpointId).First();
                        cpL.Add(cp);
                    }
                }
                return cpL;
            }

            cpL.Add(cp1);
            var checkpoints = _db.ShipmentCheckpoints.Where(x => (x.checkpointId != cp1.checkpointId) && (x.checkpointId != cp2.checkpointId)).Select(x => x).ToList();
            route(cp1,cp2,checkpoints,0);
            if (test2.Count != 0)
            {
                var minKey = test2.Keys.Min();
                Console.WriteLine(minKey + " " + test2.Count);

                var cps = test2.Where(x => x.Key == minKey).Select(x => x.Value).ToList().First();
                foreach(var cp in cps)
                {
                    Console.WriteLine(cp.checkpointName);
                    cpL.Add(cp);
                }
            }
            
            cpL.Add(cp2);
            RouteModel _route = new RouteModel(cp1, cp2);
            _db.Routes.Add(_route);
            int i = 1;
            foreach (var cp in cpL)
            {
                RouteCheckpointsModel RouteCp = new RouteCheckpointsModel(_route.routeId, cp.checkpointId, i);
                _db.RouteCheckpoints.Add(RouteCp);
                i++;
            }
            _db.SaveChanges();
            test2.Clear();
            temp.Clear();
            return cpL;
        }
        static Dictionary<float, List<CheckpointModel>> test2 = new Dictionary<float, List<CheckpointModel>>();
        List<CheckpointModel> temp = new List<CheckpointModel>();
        public void route(CheckpointModel cp1, CheckpointModel cp2,List<CheckpointModel> cp,float prevcost)
        {
            if(cp.Count == 0)
            {
                var l = new List<CheckpointModel>();
                foreach(var t in temp)
                {
                    l.Add(t);
                }
                var lastCost = DisCost(cp2, temp.Last());
                Console.WriteLine(l.Count + " " + (prevcost + lastCost));
                test2.Add(prevcost+lastCost,l);
                temp.Remove(temp.Last());

                return ;
            }
            var cost = DisCost(cp1,cp2);
            foreach(var checkpoint in cp)
            {
                var list = cp.Where(x => x.checkpointId != checkpoint.checkpointId).Select(x => x).ToList();
                var cost1 = DisCost(cp1,checkpoint);
                var cost2 = DisCost(cp2,checkpoint);
                if (cost1+cost2 < cost)
                {
                    temp.Add(checkpoint);
                    Console.WriteLine(checkpoint.checkpointName);
                    prevcost +=cost1;
                    route(checkpoint, cp2, list,prevcost);
                    prevcost -= cost1;
                }
                
            }
            var count = 0;
            foreach(var t in test2)
            {
                if(prevcost == t.Key)
                {
                    count++;
                }
            }
            if(count== 0 && temp.Count >0)
            {
                var l = new List<CheckpointModel>();
                foreach (var t in temp)
                {
                    l.Add(t);
                }
                var lastCost = DisCost(cp2, l.Last());
                Console.WriteLine(l.Count + " " +(prevcost + lastCost));
                test2.Add(prevcost + lastCost, l);
                temp.Remove(temp.Last());
            }
            
            return ;
        }
    }
}
