using ShippingApp.Data;
using ShippingApp.Models;

namespace ShippingApp.Services
{
    public class ShortestRoute : IShortestRoute
    {
        //creating a DB instance
        private readonly shipmentAppDatabase _db;
        
        // creating a static dictionary to store route according to weight
		static Dictionary<float, List<CheckpointModel>> test2 = new Dictionary<float, List<CheckpointModel>>();
        
        // variable to store route
		List<CheckpointModel> temp = new List<CheckpointModel>();
		
        // constructor
		public ShortestRoute(shipmentAppDatabase _db)
        {
            this._db = _db;
        }

        //------------ A function to get weight of route------------>
        public float DisCost(CheckpointModel cp1, CheckpointModel cp2)
        {
            //Query to get get weight b/w two checkpoints
            var costs = _db.CheckpointMappings.Where(x => (x.checkpoint1Id == cp1.checkpointId && x.checkpoint2Id == cp2.checkpointId) || (x.checkpoint1Id == cp2.checkpointId && x.checkpoint2Id == cp1.checkpointId)).Select(x => x.cost).ToList();
            
            return costs.First();
        }

        //--------------- Finding Shortest Route------------->
        public List<CheckpointModel> shortRoute(CheckpointModel cp1, CheckpointModel cp2)
        {
            // creating checkpoint list 
            List<CheckpointModel> cpL = new List<CheckpointModel>();

            // adding origin and destination and returning if both are same
            if(cp1.checkpointId == cp2.checkpointId)
            {
                cpL.Add(cp1);
                cpL.Add(cp2);
                return cpL;
            }

            // Checking if Route already exist
            var Route = _db.Routes.Where(x => ((x.checkpoint1Id == cp1.checkpointId) && (x.checkpoint2Id == cp2.checkpointId)) || ((x.checkpoint2Id == cp1.checkpointId) && (x.checkpoint1Id == cp2.checkpointId))).Select(x => x).ToList();
            
            //if yes
            if (Route.Count > 0)
            {
                //for forward route
                if (Route.First().checkpoint1Id == cp1.checkpointId && Route.First().checkpoint2Id == cp2.checkpointId)
                {
                    //getting checkpoints of the route
                    var rcps = _db.RouteCheckpoints.Where(x => x.routeId == Route.First().routeId).Select(x => x).OrderBy(x => x.index).ToList();

                    foreach (var rcp in rcps)
                    {
                        //adding Checkpoints to the list
                        var cp = _db.ShipmentCheckpoints.Where(x => x.checkpointId == rcp.checkpointId).First();
                        cpL.Add(cp);
                    }
                }
                //for reverse route
                else
                {
					//getting checkpoints of the route
					var rcps = _db.RouteCheckpoints.Where(x => x.routeId == Route.First().routeId).Select(x => x).OrderByDescending(x => x.index).ToList();
                    
                    foreach (var rcp in rcps)
                    {
						//adding Checkpoints to the list
						var cp = _db.ShipmentCheckpoints.Where(x => x.checkpointId == rcp.checkpointId).First();
                        cpL.Add(cp);
                    }
                }
                //Returning list
                return cpL;
            }

            //adding origin
            cpL.Add(cp1);
            
            //getting all checkpoints
            var checkpoints = _db.ShipmentCheckpoints.Where(x => (x.checkpointId != cp1.checkpointId) && (x.checkpointId != cp2.checkpointId)).Select(x => x).ToList();
            
            // calling route function to create route from above checkpoints
            route(cp1,cp2,checkpoints,0);
            
            //if route is created
            if (test2.Count != 0)
            {
                // finding route with minimum weight
                var minKey = test2.Keys.Min();

                //getting route
                var cps = test2.Where(x => x.Key == minKey).Select(x => x.Value).ToList().First();
                foreach(var cp in cps)
                {
                    //adding checkpoint to the list
                    cpL.Add(cp);
                }
            }
            
            //adding destination
            cpL.Add(cp2);

            //saving route in DB
            RouteModel _route = new RouteModel(cp1, cp2);
            _db.Routes.Add(_route);

            //adding route checkpoints to wthe database with indexing
            int i = 1;
            foreach (var cp in cpL)
            {
                RouteCheckpointsModel RouteCp = new RouteCheckpointsModel(_route.routeId, cp.checkpointId, i);
                _db.RouteCheckpoints.Add(RouteCp);
                i++;
            }
            _db.SaveChanges();
        
            //clearing variables
            test2.Clear();
            temp.Clear();

            return cpL;
        }

        //----------------Function to create Route------------>
        public void route(CheckpointModel cp1, CheckpointModel cp2,List<CheckpointModel> cp,float prevcost)
        {
            // if list in empty
            if(cp.Count == 0)
            {
                var l = new List<CheckpointModel>();
                foreach(var t in temp)
                {
                    l.Add(t);
                }
                var lastCost = DisCost(cp2, temp.Last());
                //add route in dictionary
                test2.Add(prevcost+lastCost,l);
                // removing last added checkpoint
                temp.Remove(temp.Last());
                return ;
            }

            //getting weight b/w two checkpoints
            var cost = DisCost(cp1,cp2);

            //iterating everycheckpoint
            foreach(var checkpoint in cp)
            {
                //getting list of checkpoinyt witout current added checkpoint
                var list = cp.Where(x => x.checkpointId != checkpoint.checkpointId).Select(x => x).ToList();
                //getting weight
                var cost1 = DisCost(cp1,checkpoint);
                var cost2 = DisCost(cp2,checkpoint);

                //checking if total weight is less than calculated weight
                if (cost1+cost2 < cost)
                {
                    //adding checkpoint in route
                    temp.Add(checkpoint);
                    //updating weight
                    prevcost +=cost1;
                    //applying recursion
                    route(checkpoint, cp2, list,prevcost);
                    //updating cost
                    prevcost -= cost1;
                }
            }
            //checking if route already exist in dictionary
            var count = 0;
            foreach(var t in test2)
            {
                if(prevcost == t.Key)
                {
                    count++;
                }
            }
            //if not then 
            if(count== 0 && temp.Count >0)
            {

                var l = new List<CheckpointModel>();
                foreach (var t in temp)
                {
                    l.Add(t);
                }
                var lastCost = DisCost(cp2, l.Last());
                //adding route in dictionary
                test2.Add(prevcost + lastCost, l);
                //removing checkpoint from route
                temp.Remove(temp.Last());
            }
            return ;
        }
    }
}
