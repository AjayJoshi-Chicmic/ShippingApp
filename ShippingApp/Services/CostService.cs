using ShippingApp.Data;
using ShippingApp.Models;

namespace ShippingApp.Services
{
    public class CostService:ICostService
    {
        //variables
        private readonly shipmentAppDatabase _db;
        private readonly IShortestRoute shortestRoute;
        private readonly IApiCallingService apiCallingService;

        //constructor with DI
        public CostService(shipmentAppDatabase _db, IShortestRoute shortestRoute,IApiCallingService apiCallingService)
        {
            this._db = _db;
            this.shortestRoute = shortestRoute;
            this.apiCallingService = apiCallingService;
        }

        //----------- A Function to calculate cost of shipment-------->>
        public ResponseModel shipmentCost(Guid cp1Id,Guid cp2Id, Guid productTypeId, Guid containerTypeId,float weight )
        {
            // get origin of the shipment
            var origin = _db.ShipmentCheckpoints.Where(x=>x.checkpointId== cp1Id).FirstOrDefault();
            // get shipment
            var destination = _db.ShipmentCheckpoints.Where(x => x.checkpointId == cp2Id).FirstOrDefault();
            //getting container type
            var container = apiCallingService.GetContainerType(containerTypeId);
            //gwtting product type
            var product = apiCallingService.GetProductType(productTypeId);
            //route
            List<GetCheckpointModel> shortRoute = shortestRoute.shortRoute(origin!,destination!);
            double totalDistance = 0;
            //calculating distance
            for(int i = 0; i < shortRoute.Count-1; i++)
            {
                totalDistance += apiCallingService.GetDistance(shortRoute[i], shortRoute[i + 1]);
            }
            //calculating cost
            var cost = (totalDistance)*5 + (weight * container.price * product.price)*2;
            return new ResponseModel("cost of shipment", cost);
            
        }
    }
}
