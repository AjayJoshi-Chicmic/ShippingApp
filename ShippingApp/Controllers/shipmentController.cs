using Microsoft.AspNetCore.Http;
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

        public shipmentController(IShipmentService shipmentService, IShortestRoute shortestRoute,ICheckpointServices checkpointServices)
        {
            this.shipmentService = shipmentService;
            this.shortestRoute = shortestRoute;
            this.checkpointServices = checkpointServices;
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
            var response = shortestRoute.bestRoute(shipment.cp1!, shipment.cp2!);
            return Ok(response);
        }
        [HttpPost]
        [Route("addCheckpoint")]
        public IActionResult addCheckpoint(AddCheckpointModel checkpoint)
        {
            var response = checkpointServices.addCheckpoint(checkpoint);
            return Ok(response);
        }
        [HttpGet]
        [Route("getCheckpoint")]
        public IActionResult getCheckpoint(Guid checkpointId)
        {
            var response = checkpointServices.getCheckpoints(checkpointId);
            return Ok(response);
        }
    }
}
