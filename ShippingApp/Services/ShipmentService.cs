using ShippingApp.Data;
using ShippingApp.Models;
using System.Text.Json;

namespace ShippingApp.Services
{
    public class ShipmentService : IShipmentService
    {
        private readonly IMessageProducerService messageProducer;
        private readonly shipmentAppDatabase _db;
        private readonly IShortestRoute shortestRoute;
        private readonly IApiCallingService apiCallingService;

        public ShipmentService(shipmentAppDatabase _db,IShortestRoute shortestRoute,IMessageProducerService messageProducer,IApiCallingService apiCallingService)
        {
            this.messageProducer = messageProducer;
            this._db= _db;
            this.shortestRoute = shortestRoute;
            this.apiCallingService = apiCallingService;
        }
        public ResponseModel AddShipment(AddShipmentModel shipment)
        {
            try
            {
                ShipmentModel _shipment = new ShipmentModel(shipment);
                ShipmentStatusModel shipmentStatus = new ShipmentStatusModel(_shipment.shipmentId);
                _db.ShipmentStatus.Add(shipmentStatus);
                _shipment.shipmentStatusId = shipmentStatus.shipmentStatusId;
                _shipment.shipmentPrice = _shipment.shipmentWeight * 83;
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
                    var product = apiCallingService.GetProductType(shipment.productTypeId);
                    var container = apiCallingService.GetContainerType(shipment.cointainerTypeId);
                    var originName = _db.ShipmentCheckpoints.Where(x => x.checkpointId == shipment.origin).Select(x => x.checkpointName).First().ToString();
                    var destinationName = _db.ShipmentCheckpoints.Where(x => x.checkpointId == shipment.destination).Select(x => x.checkpointName).First().ToString();
                    ShipmentStatusModel status = _db.ShipmentStatus.Where(x => x.shipmentStatusId==shipment.shipmentStatusId).Select(x => x).OrderByDescending(x => x.lastUpdated).First();
                    string location = "";
                    if(status.currentLocation == Guid.Empty)
                    {
                        location = "At Origin"; 
                    }
                    else
                    {
                        location = _db.ShipmentCheckpoints.Where(x => x.checkpointId == status.currentLocation).Select(x => x.checkpointName).First().ToString();

                    }
                    GetShipmentModel _shipment = new GetShipmentModel(shipment,status,location,originName ,destinationName, product.type,container.containerName);
                    models.Add(_shipment);
                } 
                return new ResponseModel("Shipments ", models);
            }
            catch (Exception ex)
            {
                return new ResponseModel(500, ex.Message + ex.StackTrace!, false);
            }
        }
        public ResponseModel ChangeStatus(ShipmentStatusModel status)
        {
            try
            {
                var shipment = _db.Shipments.Where(x => x.shipmentId == status.shipmentId).Select(x => x).ToList();
                shipment.First().shipmentStatusId = status.shipmentStatusId;
                shipment.First().lastUpdated= status.lastUpdated;
                _db.ShipmentStatus.Add(status);
                _db.SaveChanges();
                return new ResponseModel("status Changed");
            }
            catch(Exception ex)
            {
                return new ResponseModel(500, ex.Message, false);
            } 
        }
        public ResponseModel getShipmentStatus(Guid shipmentId)
        {
            try
            {
                List<GetShipmentStatus> statusList = new List<GetShipmentStatus>();
                var shipmentStatus = _db.ShipmentStatus.Where(x=>x.shipmentId == shipmentId).OrderByDescending(x=>x.lastUpdated).Select(x => x).ToList();
                foreach(var status in shipmentStatus)
                {
                    var checkpoint = _db.ShipmentCheckpoints.Where(x=>x.checkpointId==status.currentLocation).Select(x => x.checkpointName).ToList();
                    if (checkpoint.Count()==0) 
                    {
                        var Sstatus = new GetShipmentStatus(status, "At Origin");
                        statusList.Add(Sstatus);
                    }
                    else
                    {
                        var Sstatus = new GetShipmentStatus(status, checkpoint.First());
                        statusList.Add(Sstatus);
                    }
                }
                return new ResponseModel("Shipment History",statusList);
            }
            catch(Exception ex)
            {
                return new ResponseModel(500, ex.Message, false);
            }
        }
        public ResponseModel getDriverShipment(Guid driverId) 
        {
            try
            {
                List<GetShipmentModel> shipments = new List<GetShipmentModel>();   
                var shipmentIds=apiCallingService.GetShipmentId(driverId);
                foreach(var shipmentId in shipmentIds)
                {
                    var id = new Guid(shipmentId);
                    var res = GetShipment(id, Guid.Empty, Guid.Empty, Guid.Empty);

                    var obj = JsonSerializer.Serialize(res.data);
                    List<GetShipmentModel> model = JsonSerializer.Deserialize<List<GetShipmentModel>>(obj!)!;
                    shipments.Add(model.First());
                }
                return new ResponseModel(shipments);
            }
            catch(Exception ex)
            {
                return new ResponseModel(500, ex.Message , false);
            } 
        }
        public ResponseModel getShortRoute(Guid shipmentId)
        {
            var shipment = _db.Shipments.Where(x=>x.shipmentId== shipmentId).First();
            var origin = _db.ShipmentCheckpoints.Where(y=>y.checkpointId == shipment.origin).Select(x=>x).FirstOrDefault();
            var destination = _db.ShipmentCheckpoints.Where(y => y.checkpointId == shipment.destination).Select(x => x).FirstOrDefault();
            var res = shortestRoute.bestRoute(origin, destination);
            return new ResponseModel(res);
        }
    }
}
