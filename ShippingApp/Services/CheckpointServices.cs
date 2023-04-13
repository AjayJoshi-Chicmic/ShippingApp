using ShippingApp.Data;
using ShippingApp.Migrations;
using ShippingApp.Models;

namespace ShippingApp.Services
{
    public class CheckpointServices : ICheckpointServices
    {
        private readonly shipmentAppDatabase _db;
        public CheckpointServices(shipmentAppDatabase _db)
        {
            this._db = _db;
        }
        public ResponseModel addCheckpoint(AddCheckpointModel checkpoint)
        {
            var _checkpoint = new CheckpointModel(checkpoint);
            _db.ShipmentCheckpoints.Add(_checkpoint);
            _db.SaveChanges();
            createDistance(_checkpoint);
            _db.SaveChanges();
            return new ResponseModel();
        }
        public void createDistance(CheckpointModel checkpoint)
        {
            var checkpoints = _db.ShipmentCheckpoints.Select(x => x).ToList();
            if(checkpoints.Count == 0) { return; }
            foreach (var _checkpoint in checkpoints)
            {
                const double earthRadius = 6371; // kilometers

                var lat1 = checkpoint.latitude;
                var lon1 = checkpoint.longitude;
                var lat2 = _checkpoint.latitude;
                var lon2 = _checkpoint.longitude;

                var dLat = (lat2 - lat1) * Math.PI / 180.0;
                var dLon = (lon2 - lon1) * Math.PI / 180.0;

                var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                        Math.Cos(lat1 * (Math.PI / 180.0)) * Math.Cos(lat2 * (Math.PI / 180.0)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

                var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
                double dist = earthRadius * c;
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
    }
}
