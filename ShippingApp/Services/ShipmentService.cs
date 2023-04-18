using ShippingApp.Data;
using ShippingApp.Models;

namespace ShippingApp.Services
{
    public class ShipmentService : IShipmentService
    {
        private readonly IMessageProducerService messageProducer;
        private readonly shipmentAppDatabase _db;
        private readonly IShortestRoute shortestRoute;

        public ShipmentService(shipmentAppDatabase _db,IShortestRoute shortestRoute,IMessageProducerService messageProducer)
        {
            this.messageProducer = messageProducer;
            this._db= _db;
            this.shortestRoute = shortestRoute;
        }
        public ResponseModel AddShipment(AddShipmentModel shipment)
        {
            try
            {
                ShipmentModel _shipment = new ShipmentModel(shipment);
                ShipmentStatusModel shipmentStatus = new ShipmentStatusModel(_shipment.shipmentId);
                _db.ShipmentStatus.Add(shipmentStatus);
                _shipment.shipmentStatusId = shipmentStatus.shipmentStatusId;
                _db.Shipments.Add(_shipment);
                _db.SaveChanges();
                var cpt = new GetShipmentRoute();
                var cp1 = _db.ShipmentCheckpoints.Where(x=>x.checkpointId == shipment.origin).First();
                var cp2 = _db.ShipmentCheckpoints.Where(x => x.checkpointId == shipment.destination).First();
                var res2 = shortestRoute.bestRoute(cp1, cp2);
                var del = new ShipmentDeliveryModel
                {
                    shipment = _shipment,
                    checkpoints = res2
                };
                messageProducer.producer("shipmentDelivery", del);
                return new ResponseModel("Shipment Added",_shipment);
            }
            catch(Exception ex)
            { 
                return new ResponseModel(500, ex.Message, false); ;
            }
        }
        public ResponseModel GetShipment(Guid shipmentId,Guid customerId,Guid productTypeId,Guid containerTypeId)
        {
            try
            {
                var shipments = _db.Shipments.Where(x => (x.shipmentId == shipmentId || shipmentId == Guid.Empty) && (x.customerId == customerId || customerId == Guid.Empty) && (x.productTypeId == productTypeId || productTypeId == Guid.Empty) && (x.cointainerTypeId == containerTypeId || containerTypeId == Guid.Empty)).Select(x => x).ToList();
                if(shipments.Count == 0 )
                {
                    return new ResponseModel(404,"Shipment Not Found",false);
                }
                List<GetShipmentModel> models = new List<GetShipmentModel>();   
                foreach(var shipment in shipments)
                {
                    ShipmentStatusModel status = _db.ShipmentStatus.Where(x => x.shipmentStatusId==shipment.shipmentStatusId).Select(x => x).OrderByDescending(x => x.lastUpdated).First();
                    GetShipmentModel _shipment = new GetShipmentModel(shipment,status);
                    models.Add(_shipment);
                } 
                return new ResponseModel("Shipments ", models);
            }
            catch (Exception ex)
            {
                return new ResponseModel(500, ex.Message, false);
            }
        }
        public ResponseModel ChangeStatus(ShipmentStatusModel status)
        {
            try
            {
                var shipment = _db.Shipments.Where(x => x.shipmentId == status.shipmentId).Select(x => x).ToList();
                shipment.First().shipmentStatusId = status.shipmentStatusId;
                _db.ShipmentStatus.Add(status);
                _db.SaveChanges();
                return new ResponseModel("status Changed");
            }
            catch(Exception ex)
            {
                return new ResponseModel(500, ex.Message, false);
            } 
        }
    }
}
