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

        public shipmentController(IShipmentService shipmentService)
        {
            this.shipmentService = shipmentService;
        }
        [HttpGet]
        public IActionResult getShipment(Guid shipmentId,Guid customerId,Guid productTypeId,Guid containerTypeId)
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
    }
}
