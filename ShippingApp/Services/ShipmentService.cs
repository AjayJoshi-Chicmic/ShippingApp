using ShippingApp.Data;
using ShippingApp.Models;

namespace ShippingApp.Services
{
    public class ShipmentService : IShipmentService
    {
        private readonly shipmentAppDatabase _db;
        
        public ShipmentService(shipmentAppDatabase _db)
        {
            this._db = _db;
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
                //var res = messageQueue.producer("test",_shipment);
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
    }
}
