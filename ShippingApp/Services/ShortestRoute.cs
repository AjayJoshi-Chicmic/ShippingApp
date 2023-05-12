using ShippingApp.Data;
using ShippingApp.Models;

namespace ShippingApp.Services
{
    public class ShortestRoute : IShortestRoute
    {
        //creating a DB instance
        private readonly shipmentAppDatabase _db;
        
        // constructor
		public ShortestRoute(shipmentAppDatabase _db)
        {
            this._db = _db;
        }

        //--------------- Finding Shortest Route------------->
        public List<GetCheckpointModel> shortRoute(CheckpointModel cp1, CheckpointModel cp2)
        {
            // creating checkpoint list 
            List<GetCheckpointModel> cpL = new List<GetCheckpointModel>();

            // adding origin and destination and returning if both are same
            if(cp1.checkpointId == cp2.checkpointId)
            {
                cpL.Add(new GetCheckpointModel(cp1));
                cpL.Add(new GetCheckpointModel(cp2));
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
                        cpL.Add(new GetCheckpointModel(cp));
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
                        cpL.Add(new GetCheckpointModel(cp));
                    }
                }
                //Returning list
                return cpL;
            }

			List<CheckpointModel> cpList = new List<CheckpointModel>();
			var cp1H = checkpointHierarchy(cp1);
			var cp2H = checkpointHierarchy(cp2);

            var check = 0;
            int k = 0;
            foreach (var cp in cp1H)
            {
                if(check == 0)
                {
					cpList.Add(cp);
					for (int i = 0; i < cp2H.Count; i++)
					{
						if (cp.checkpointId == cp2H[i].checkpointId)
						{
							check = i;
                            if(i!=0 && k != 0)
                            {
								cpList.Remove(cp);
							}
							break;
						}
					}
				}
                k++;
            }
            for(int i = check-1; i >= 0; i--)
            {
                cpList.Add(cp2H[i]);
			}
            var route = new RouteModel(cp1, cp2);
            _db.Routes.Add(route);
            int j = 0;
            foreach(var cp in cpList)
            {
                j++;
                _db.RouteCheckpoints.Add(new RouteCheckpointsModel(route.routeId, cp.checkpointId, j));
                cpL.Add(new GetCheckpointModel(cp));
            }
            _db.SaveChanges();
			return cpL;
        }
        public List<CheckpointModel> checkpointHierarchy(CheckpointModel cp)
        {
            List<CheckpointModel> cpL = new List<CheckpointModel> ();
            int i = 0;
			while (i==0)
            {
				cpL.Add(cp);
				var checkpoint = _db.ShipmentCheckpoints.Where(x => (x.checkpointId == cp.parentCheckpointId)).Select(x => x).ToList();
                if(checkpoint.Count != 0)
                {
					cp = checkpoint.First();
				}
                else
                {
                    i = 1;
                }
			}
            return cpL;
        }
    }
}
