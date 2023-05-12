using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using ShippingApp.Data;
using ShippingApp.Migrations;
using ShippingApp.Models;
using System.Collections.Generic;

namespace ShippingApp.Services
{
    public class CheckpointServices : ICheckpointServices
    {
        // variables
        private readonly shipmentAppDatabase _db;
        private readonly IApiCallingService apiCalling;
        // constructor with DI
        public CheckpointServices(shipmentAppDatabase _db,IApiCallingService apiCalling)
        {
            this._db = _db;
            this.apiCalling = apiCalling;
        }

        //-------------A Function to add checkpoint in DB---------->
        public ResponseModel addCheckpoint(AddCheckpointModel checkpoint)
        {
            try
            {
                //getting checkpoint with same name and lat long
				var cp = _db.ShipmentCheckpoints.Where(x => (x.checkpointName == checkpoint.checkpointName) ||(x.latitude==checkpoint.latitude && x.longitude == checkpoint.longitude)).Count();
                //if checkpoint already exist
                if(cp != 0)
                {
					return new ResponseModel(400,"Checkpoint already exist",false);
				}
				//creating a checkpoint instance
				var _checkpoint = new CheckpointModel(checkpoint);
                
                //adding checkpoint in DB
                _db.ShipmentCheckpoints.Add(_checkpoint);
                //saving checkpoint
                _db.SaveChanges();
                //calling a function to create connection with existing checkpoints
                createDistance(_checkpoint);
                //saving Db
                _db.SaveChanges();
                //removing all existing routes to make new routes with new checkpoint
                var count = _db.Routes.Count();
                _db.Routes.RemoveRange(_db.Routes);
                _db.SaveChanges();
                _db.RouteCheckpoints.RemoveRange(_db.RouteCheckpoints);
                _db.SaveChanges();
                return new ResponseModel("checkpoint added");
            }
            catch(Exception ex)
            {
                return new ResponseModel(500,ex.Message,false);
            }
            
        }

        //------------------ A Function to create connection between a new checkpoint and already existing checkpoints-----------.
        public void createDistance(CheckpointModel checkpoint)
        {
            // getting all checkpoints
            var checkpoints = _db.ShipmentCheckpoints.Where(x=>x.checkpointId==checkpoint.parentCheckpointId).Select(x => x).ToList();
            // if no checkpoint exist
            if(checkpoints.Count == 0) { return; }

			double dist = apiCalling.GetDistance(new GetCheckpointModel(checkpoint), new GetCheckpointModel(checkpoints.First()));
			var checkpointDistanceMap = new CheckpointsDistanceModel
			{
				checkpoint1Id = checkpoint.checkpointId,
				checkpoint2Id = checkpoints.First().checkpointId,
				distance = Convert.ToSingle(dist),
				cost = 0
			};
			_db.CheckpointMappings.Add(checkpointDistanceMap);
            var siblings = _db.ShipmentCheckpoints.Where(x => x.parentCheckpointId == checkpoint.parentCheckpointId && x.checkpointId != checkpoint.checkpointId).Select(x => x).ToList();
            foreach (var s in siblings)
            {
				double distance = apiCalling.GetDistance(new GetCheckpointModel(checkpoint), new GetCheckpointModel(s));
				var checkpointDistanceMap1 = new CheckpointsDistanceModel
				{
					checkpoint1Id = checkpoint.checkpointId,
					checkpoint2Id = s.checkpointId,
					distance = Convert.ToSingle(distance),
					cost = 0
				};
				_db.CheckpointMappings.Add(checkpointDistanceMap1);
			}
        }

        //---------------- A Function to get checkpoints------------------->
        public ResponseModel getCheckpoints(Guid checkpointId,string checkpointName)
        {
            try
            {
                //getting checkpoint according to ID and name
                var checkpoints = _db.ShipmentCheckpoints.Where(x => (x.checkpointId == checkpointId || checkpointId == Guid.Empty)&&(EF.Functions.Like(x.checkpointName, "%" + checkpointName + "%")|| checkpointName == null)).Select(x => x).ToList();
                //IF no checkpoint found
                if(checkpoints.Count== 0)
                {
                    //returning no checkpoint found
                    return new ResponseModel(404,"no checkpoint found", checkpoints,false);
                }
                List<GetCheckpointModel> list = new List<GetCheckpointModel>();
                foreach (var cp in checkpoints)
                {
                    list.Add(new GetCheckpointModel(cp));
                }
                //returning checkpoints
                return new ResponseModel("Checkpoint", list);
            }
            catch(Exception ex)
            {
                return new ResponseModel(500, ex.Message, false);
            }
        }

        //------------ A Function to get distance between two checkpoints----->
        public ResponseModel getDistance(Guid cp1Id,Guid cp2Id)
        {
            try
            {
                //Getting distance from database
                var dist = _db.CheckpointMappings.Where(x=>(x.checkpoint1Id == cp1Id && x.checkpoint2Id == cp2Id)|| (x.checkpoint2Id == cp1Id && x.checkpoint1Id == cp2Id)).Select(x=>x.distance);
                //If no mapping between checkpoints found
                if(dist.Count() == 0)
                {
                    //returning error
                    return new ResponseModel(404, "No coonection Found",false);
                }
                return new ResponseModel("distance",dist.First());
            }
            catch(Exception ex)
            {
                return new ResponseModel(500, ex.Message, false);
            }
        }
        public ResponseModel AddGrandParent(AddCheckpointModel gp)
        {
            gp.parentCheckpointId = Guid.Empty;
            _db.ShipmentCheckpoints.Add(new CheckpointModel(gp));
            _db.SaveChanges();
			return new ResponseModel();
		}
    }
}
