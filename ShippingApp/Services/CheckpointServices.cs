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

        public CheckpointServices(shipmentAppDatabase _db,IApiCallingService apiCalling)
        {
            this._db = _db;
            this.apiCalling = apiCalling;
        }
        public ResponseModel addCheckpoint(AddCheckpointModel checkpoint)
        {
            try
            {
                var _checkpoint = new CheckpointModel(checkpoint);
                _db.ShipmentCheckpoints.Add(_checkpoint);
                _db.SaveChanges();
                createDistance(_checkpoint);
                _db.SaveChanges();
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
        public void createDistance(CheckpointModel checkpoint)
        {
            var checkpoints = _db.ShipmentCheckpoints.Where(x=>x.checkpointId!=checkpoint.checkpointId).Select(x => x).ToList();
            if(checkpoints.Count == 0) { return; }
            foreach (var _checkpoint in checkpoints)
            {
                double dist = apiCalling.GetDistance(checkpoint, _checkpoint);
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
                var checkpointDistanceMap = new CheckpointsDistanceModel
                {
                    checkpoint1Id = checkpoint.checkpointId,
                    checkpoint2Id = _checkpoint.checkpointId,
                    distance = Convert.ToSingle(dist),
                    cost = Convert.ToSingle(cost)
                };
                _db.CheckpointMappings.Add(checkpointDistanceMap);
            }
        }
        public ResponseModel getCheckpoints(Guid checkpointId,string checkpointName)
        {
            try
            {
                var checkpoints = _db.ShipmentCheckpoints.Where(x => (x.checkpointId == checkpointId || checkpointId == Guid.Empty)&&(EF.Functions.Like(x.checkpointName, "%" + checkpointName + "%")|| checkpointName == null)).Select(x => x).ToList();
                if(checkpoints.Count== 0)
                {
                    return new ResponseModel(404,"Checkpoint not found", checkpoints,false);
                }

                return new ResponseModel("Checkpoint", checkpoints);
            }
            catch(Exception ex)
            {
                return new ResponseModel(500, ex.Message, false);
            }
        }
        public ResponseModel getDistance(Guid cp1Id,Guid cp2Id)
        {
            try
            {
                var dist = _db.CheckpointMappings.Where(x=>(x.checkpoint1Id == cp1Id && x.checkpoint2Id == cp2Id)|| (x.checkpoint2Id == cp1Id && x.checkpoint1Id == cp2Id)).Select(x=>x.distance);
                if(dist.Count() == 0)
                {
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
