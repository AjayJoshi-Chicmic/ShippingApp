using ShippingApp.Data;
using ShippingApp.Models;

namespace ShippingApp.Services
{
    public class CostService:ICostService
    {
        private readonly shipmentAppDatabase _db;
        private readonly IShortestRoute shortestRoute;
        private readonly IApiCallingService apiCallingService;

        public CostService(shipmentAppDatabase _db, IShortestRoute shortestRoute,IApiCallingService apiCallingService)
        {
            this._db = _db;
            this.shortestRoute = shortestRoute;
            this.apiCallingService = apiCallingService;
        }
        public ResponseModel shipmentCost(Guid cp1Id,Guid cp2Id, Guid productTypeId, Guid containerTypeId,float weight )
        {
            var origin = _db.ShipmentCheckpoints.Where(x=>x.checkpointId== cp1Id).FirstOrDefault();
            var destination = _db.ShipmentCheckpoints.Where(x => x.checkpointId == cp2Id).FirstOrDefault();

            var container = apiCallingService.GetContainerType(containerTypeId);
            var product = apiCallingService.GetProductType(productTypeId);
            List<CheckpointModel> shortRoute = shortestRoute.shortRoute(origin!,destination!);
            double totalDistance = 0;
            for(int i = 0; i < shortRoute.Count-1; i++)
            {
                totalDistance += apiCallingService.GetDistance(shortRoute[i], shortRoute[i + 1]);
            }
            var cost = (totalDistance + weight * container.price * product.price)*5;
            return new ResponseModel("cost of shipment", cost);
            
        }
    }
}
