using Microsoft.EntityFrameworkCore;
using ShippingApp.Data;
using ShippingApp.Migrations;
using ShippingApp.Models;

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
                return new ResponseModel();
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
            var checkpoints = _db.ShipmentCheckpoints.Where(x=>x.checkpointId!=checkpoint.checkpointId).Select(x => x).ToList();
            // if no checkpoint exist
            if(checkpoints.Count == 0) { return; }

            foreach (var _checkpoint in checkpoints)
            {
                //calling a function to get distance between two checkpoints
                double dist = apiCalling.GetDistance(checkpoint, _checkpoint);
                //calculating weight for each connection
                double cost = 0;
                if (dist < 50)
                {
                    cost = dist * 5;
                }
                else if (dist >= 50 && dist < 100)
                {
                    cost = dist * 10;
                }
                else if (dist >= 100 && dist < 300)
                {
                    cost = dist * 20;
                }
                else
                {
                    cost = dist * 40;
                }
                //creating an instance of the connection
                var checkpointDistanceMap = new CheckpointsDistanceModel
                {
                    checkpoint1Id = checkpoint.checkpointId,
                    checkpoint2Id = _checkpoint.checkpointId,
                    distance = Convert.ToSingle(dist),
                    cost = Convert.ToSingle(cost)
                };
				//saving this connection in database
				_db.CheckpointMappings.Add(checkpointDistanceMap);
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
                //returning checkpoints
                return new ResponseModel("Checkpoint", checkpoints);
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
                    return new ResponseModel(404, "Not Found",false);
                }
                return new ResponseModel("distance",dist.First());
            }
            catch(Exception ex)
            {
                return new ResponseModel(500, ex.Message, false);
            }
        }
    }
}
