using ShippingApp.Data;
using ShippingApp.Models;
using System.Text.Json;

namespace ShippingApp.Services
{
    public class ShipmentService : IShipmentService
    {
        //creating instances of services
        private readonly IMessageProducerService messageProducer;
        private readonly shipmentAppDatabase _db;
        private readonly IShortestRoute shortestRoute;
        private readonly IApiCallingService apiCallingService;
        private readonly ICostService costService;

        //construction with dependency injection
        public ShipmentService(shipmentAppDatabase _db,IShortestRoute shortestRoute,IMessageProducerService messageProducer,IApiCallingService apiCallingService,ICostService costService)
        {
            this.messageProducer = messageProducer;
            this._db= _db;
            this.shortestRoute = shortestRoute;
            this.apiCallingService = apiCallingService;
            this.costService = costService;
        }

        //------------A Function to create shipment
        public ResponseModel AddShipment(AddShipmentModel shipment)
        {
            try
            {
                //creating a shipment entity
                ShipmentModel _shipment = new ShipmentModel(shipment);
                //cretaing shipmnet status
                ShipmentStatusModel shipmentStatus = new ShipmentStatusModel(_shipment.shipmentId);
                //Shipment transaction map 
                ShipmentPaymentMap shipmentPaymentMap = new ShipmentPaymentMap(_shipment.shipmentId,shipment.transactionRecordId);
                //saving payment maping in DB
                _db.shipmentPaymentMaps.Add(shipmentPaymentMap);
                //saving status in DB
                _db.ShipmentStatus.Add(shipmentStatus);
                //updating shipment
                _shipment.shipmentStatusId = shipmentStatus.shipmentStatusId;
                //calculating cost
                var cost = costService.shipmentCost(shipment.origin,shipment.destination,shipment.productTypeId,shipment.containerTypeId,shipment.shipmentWeight);
                //saving cost in DB
                _shipment.shipmentPrice = (JsonSerializer.Deserialize<float>(JsonSerializer.Serialize(cost.data)));
                _shipment.shipmentPrice =  _shipment.shipmentPrice * 1.18f;
                _db.Shipments.Add(_shipment);
                _db.SaveChanges();

                //response data
                var cpt = new GetShipmentRoute();
                var cp1 = _db.ShipmentCheckpoints.Where(x=>x.checkpointId == shipment.origin).First();
                var cp2 = _db.ShipmentCheckpoints.Where(x => x.checkpointId == shipment.destination).First();
                //finding Route
                var res2 = shortestRoute.shortRoute(cp1, cp2);
                var ship = new SendShipmentModel();
                ship.shipmentId = _shipment.shipmentId;
                ship.shipmentWeight = _shipment.shipmentWeight;
                ship.origin = _shipment.origin;
                ship.Destination = _shipment.destination;
                //getting product type
                var product = apiCallingService.GetProductType(_shipment.productTypeId);
                ship.productType = product.type;
                //getting containertype
                var container = apiCallingService.GetContainerType(_shipment.cointainerTypeId);
                ship.containerType = container.containerName;
                var del = new ShipmentDeliveryModel
                {
                    shipment = ship,
                    checkpoints = res2
                };
                // adding delivery to queue
                messageProducer.producer("shipmentDelivery", del);
                return new ResponseModel("Shipment Added",_shipment);
            }
            catch(Exception ex)
            { 
                return new ResponseModel(500, ex.Message, false); ;
            }
        }

        //-------------A function to GetShipment-------->
        public ResponseModel GetShipment(Guid shipmentId,Guid customerId,Guid productTypeId,Guid containerTypeId)
        {
            try
            {
                //fetching shipment from DB
                var shipments = _db.Shipments.Where(x => (x.shipmentId == shipmentId || shipmentId == Guid.Empty) && (x.customerId == customerId || customerId == Guid.Empty) && (x.productTypeId == productTypeId || productTypeId == Guid.Empty) && (x.cointainerTypeId == containerTypeId || containerTypeId == Guid.Empty)).Select(x => x).ToList();
                //if no shipment exist
                if(shipments.Count == 0 )
                {
                    return new ResponseModel(404,"Shipment Not Found",false);
                }
                //initializing get shipment model
                List<GetShipmentModel> models = new List<GetShipmentModel>();   
                // foreach shipment
                foreach(var shipment in shipments)
                {
                    //getting product type
                    var product = apiCallingService.GetProductType(shipment.productTypeId);
                    //getting container type
                    var container = apiCallingService.GetContainerType(shipment.cointainerTypeId);
                    //getting origin name
                    var originName = _db.ShipmentCheckpoints.Where(x => x.checkpointId == shipment.origin).Select(x => x.checkpointName).First().ToString();
                    //getting destination name
                    var destinationName = _db.ShipmentCheckpoints.Where(x => x.checkpointId == shipment.destination).Select(x => x.checkpointName).First().ToString();
                    //getting shipment status
                    ShipmentStatusModel status = _db.ShipmentStatus.Where(x => x.shipmentStatusId==shipment.shipmentStatusId).Select(x => x).OrderByDescending(x => x.lastUpdated).First();
                    string location = "";
                    if(status.currentLocation == Guid.Empty)
                    {
                        location = "At Origin"; 
                    }
                    else
                    {
                        //adding current location
                        location = _db.ShipmentCheckpoints.Where(x => x.checkpointId == status.currentLocation).Select(x => x.checkpointName).First().ToString();
                    }
                    GetShipmentModel _shipment = new GetShipmentModel(shipment,status,location,originName ,destinationName, product.type,container.containerName);
                    //Adding shipment in list
                    models.Add(_shipment);
                } 
                return new ResponseModel("Shipments ", models);
            }
            catch (Exception ex)
            {
                return new ResponseModel(500, ex.Message + ex.StackTrace!, false);
            }
        }
        //--------------- A Function to Change status ------>
        public ResponseModel ChangeStatus(ShipmentStatusModel status)
        {
            try
            {
                //getting shipment
                var shipment = _db.Shipments.Where(x => x.shipmentId == status.shipmentId).Select(x => x).ToList();
                //updating shipment
                shipment.First().shipmentStatusId = status.shipmentStatusId;
                shipment.First().lastUpdated= status.lastUpdated;
                //adding status to DB
                _db.ShipmentStatus.Add(status);
                _db.SaveChanges();
                return new ResponseModel("status Changed");
            }
            catch(Exception ex)
            {
                return new ResponseModel(500, ex.Message, false);
            } 
        }
        //--------------- A Function to Get Shipment Status------------>
        public ResponseModel getShipmentStatus(Guid shipmentId)
        {
            try
            {
                //creating a status list
                List<GetShipmentStatus> statusList = new List<GetShipmentStatus>();
                //getting shipment status
                var shipmentStatus = _db.ShipmentStatus.Where(x=>x.shipmentId == shipmentId).OrderByDescending(x=>x.lastUpdated).Select(x => x).ToList();
                //show checkpoint name in status
                foreach(var status in shipmentStatus)
                {
                    //Getting checkpoints
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
        
        //------------- A Function to get short route ----------->
        public ResponseModel getShortRoute(Guid shipmentId)
        {
            try
            {
                //getting shipments
                var shipment = _db.Shipments.Where(x => x.shipmentId == shipmentId).First();
                //getting origin
                var origin = _db.ShipmentCheckpoints.Where(y => y.checkpointId == shipment.origin).Select(x => x).FirstOrDefault();
                //getting destination
                var destination = _db.ShipmentCheckpoints.Where(y => y.checkpointId == shipment.destination).Select(x => x).FirstOrDefault();
                //getting shortroute from origin to destination
                var res = shortestRoute.shortRoute(origin!, destination!);
                return new ResponseModel(res);
            }
            catch(Exception ex)
            {
                return new ResponseModel(500, ex.Message, false);
            }
        }

        //-------------- A Function to get shipment stats-------->>
        public ResponseModel shipmentData()
        {
            try
            {
                AdminDasboardDataModel dataModel= new AdminDasboardDataModel();
                //getting total revenue
                dataModel.totalRevenue = _db.Shipments.Select(x=>x.shipmentPrice).Sum();
                //getting total shipments 
                dataModel.totalShipment = _db.Shipments.Select(x => x).Count();
                //getting shipment of current day
                dataModel.todayShipment = _db.Shipments.Where(x => x.dateOfOrder.Date == DateTime.Now.Date).Select(x => x).Count();
                //getting count of shipment of current month
                dataModel.monthshipment = _db.Shipments.Where(x => x.dateOfOrder.Month == DateTime.Now.Month && x.dateOfOrder.Year == DateTime.Now.Year).Select(x => x.shipmentPrice).Count();
                //getting total revenue of current day
                dataModel.todayRevenue = _db.Shipments.Where(x=>x.dateOfOrder.Date==DateTime.Now.Date).Select(x => x.shipmentPrice).Sum();
                //getting this monts revenue
                dataModel.monthRevenue = _db.Shipments.Where(x => x.dateOfOrder.Month == DateTime.Now.Month&& x.dateOfOrder.Year == DateTime.Now.Year).Select(x => x.shipmentPrice).Sum();
                return new ResponseModel(dataModel);
            }
            catch (Exception ex)
            {
                return new ResponseModel(500, ex.Message, false);
            }
        }
        //--------- A Function to Get data to show in chart
        public ResponseModel chartData()
        {
            try
            {
                List<float> monthRevenue = new List<float>();
                List<int> monthShipment = new List<int>();
                //data for each month
                for (int i = 1; i <= 12; i++)
                {
                    //revenue for each month
                    var revenue = _db.Shipments.Where(t => t.dateOfOrder.Year == DateTime.Now.Year && t.dateOfOrder.Month == i).Select(t => t.shipmentPrice).Sum();
                    //total shipment of month
                    var shipment = _db.Shipments.Where(t => t.dateOfOrder.Year == DateTime.Now.Year && t.dateOfOrder.Month == i).Select(t => t.shipmentPrice).Count();
                    monthRevenue.Add(revenue);
                    monthShipment.Add(shipment);
                }

                return new ResponseModel("monthly revenue and shipment count",new ChartDataModel(monthShipment,monthRevenue));
            }
            catch (Exception ex)
            {
                return new ResponseModel(500, ex.Message, false);
            }
        }
    }
}
