using System.ComponentModel.DataAnnotations;

namespace ShippingApp.Models
{
	public class ShipmentPaymentMap
	{
		[Key]
		public Guid mapId { get; set; }= Guid.NewGuid();
		public Guid shipmentId { get; set; } = Guid.Empty;
		public Guid transactionRecordId { get; set; } = Guid.Empty;
		public ShipmentPaymentMap()
		{

		}
		public ShipmentPaymentMap(Guid shipmentId ,Guid transactionRecordId)
		{
			this.shipmentId = shipmentId;
			this.transactionRecordId = transactionRecordId;
		}
	}
}
