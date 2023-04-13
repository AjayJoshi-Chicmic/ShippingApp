using ShippingApp.Data;
using ShippingApp.Models;
using System.Collections;
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
        
        List<CheckpointModel> cps = new List<CheckpointModel>();
        //List<CheckpointModel> checkpoints = _db.Checkpoints.Select(x => x).ToList();
        public List<CheckpointModel> bestRoute(CheckpointModel cp1,CheckpointModel cp2)
        {
            List<CheckpointModel> cpList= new List<CheckpointModel>();
            var checkpoints = _db.ShipmentCheckpoints.Where(x=>(x.checkpointId != cp1.checkpointId) && (x.checkpointId != cp2.checkpointId)).Select(x => x).ToList();
            temp = checkpoints;
            cps.Add(cp1);
            var cost = getCost(cp1,cp2);
            cps.Add(cp2);
            Console.WriteLine(cps);
            return cps;
        }

        List<CheckpointModel> temp = new List<CheckpointModel>();

        public  getBestRouteCheckpoint getCost(CheckpointModel cp1, CheckpointModel cp2)
        {
            var costs = _db.CheckpointMappings.Where(x => (x.checkpoint1Id == cp1.checkpointId && x.checkpoint2Id == cp2.checkpointId) || (x.checkpoint1Id == cp2.checkpointId && x.checkpoint2Id == cp1.checkpointId)).Select(x=>x.cost).ToList();
            var temp2 = temp.Where(x => x.checkpointId != cp1.checkpointId && x.checkpointId != cp2.checkpointId).Select(x => x).ToList();
            temp = temp2;
            if (temp.Count == 0) 
            {
                return new getBestRouteCheckpoint(costs.First(),cp1,cp2);
            }
            float cost = costs.First();
            float tempCost = 0;
            var shortRoute = new getBestRouteCheckpoint(0,cp1,cp1);
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
