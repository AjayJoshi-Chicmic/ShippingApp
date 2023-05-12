using Microsoft.AspNetCore.Mvc;
using ShippingApp.Models;
using ShippingApp.Services;

namespace ShippingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class shipmentController : ControllerBase
    {
        private readonly IShipmentService shipmentService;
        private readonly IShortestRoute shortestRoute;
        private readonly ICheckpointServices checkpointServices;
        private readonly ICostService costService;

        public shipmentController(IShipmentService shipmentService, IShortestRoute shortestRoute,ICheckpointServices checkpointServices,ICostService costService)
        {
            this.shipmentService = shipmentService;
            this.shortestRoute = shortestRoute;
            this.checkpointServices = checkpointServices;
            this.costService = costService;
        }
        [HttpGet]
        public IActionResult getShipment(Guid shipmentId, Guid customerId, Guid productTypeId, Guid containerTypeId)
        {
            var response = shipmentService.GetShipment(shipmentId, customerId, productTypeId, containerTypeId);
            return StatusCode(response.statusCode, response);
        }
        [HttpPost]
        public IActionResult AddShipment(AddShipmentModel shipment)
        {
            var response = shipmentService.AddShipment(shipment);
            return StatusCode(response.statusCode, response);
        }
        [HttpPost]
        [Route("shipmentRoute")]
        public IActionResult ShipmentRoute(GetShipmentRoute shipment)
        {
            var response = shortestRoute.shortRoute(shipment.cp1!, shipment.cp2!);
            return Ok(response);
        }
        [HttpPost]
        [Route("addCheckpoint")]
        public IActionResult addCheckpoint(AddCheckpointModel checkpoint)
        {
            var response = checkpointServices.addCheckpoint(checkpoint);
            return StatusCode(response.statusCode, response);
        }
        [HttpGet]
        [Route("getCheckpoint")]
        public IActionResult getCheckpoint(Guid checkpointId, string? checkpointName)
        {
            var response = checkpointServices.getCheckpoints(checkpointId,checkpointName!);
            return StatusCode(response.statusCode, response);
        }
        [HttpGet]
        [Route("getShipmentStatus")]
        public IActionResult getShipmentStatus(Guid shipmentId)
        {
            var response = shipmentService.getShipmentStatus(shipmentId);
            return StatusCode(response.statusCode, response);
        }
        [HttpGet]
        [Route("getShipmentRoute")]
        public IActionResult getShipmentRoute(Guid shipmentId)
        {
            var response = shipmentService.getShortRoute(shipmentId);
            return StatusCode(response.statusCode, response);
        }
        [HttpPost]
        [Route("getCost")]
        public IActionResult getCost(GetCostModel shipment)
        {
            var response = costService.shipmentCost(shipment.origin,shipment.destination,shipment.productTypeId,shipment.containerTypeId,shipment.shipmentWeight);
            return StatusCode(response.statusCode, response);
        }
        [HttpGet]
        [Route("getDistance")]
        public IActionResult getdistance(Guid checkpoint1,Guid checkpoint2)
        {
            var response = checkpointServices.getDistance(checkpoint1,checkpoint2);
            return StatusCode(response.statusCode,response);
        }
        [HttpGet]
        [Route("getData")]
        public IActionResult getData()
        {
            var response = shipmentService.shipmentData();
            return StatusCode(response.statusCode, response);
        }
        [HttpGet]
        [Route("getChartData")]
        public IActionResult getChartData()
        {
            var response = shipmentService.chartData();
            return StatusCode(response.statusCode, response);
        }
		[HttpPost]
		[Route("addGrandparent")]
		public IActionResult addnode(AddCheckpointModel gp)
		{
			var response = checkpointServices.AddGrandParent(gp);
			return StatusCode(response.statusCode, response);
		}
	}
}