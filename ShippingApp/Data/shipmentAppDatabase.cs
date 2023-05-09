using Microsoft.EntityFrameworkCore;
using ShippingApp.Models;

namespace ShippingApp.Data
{
    public class shipmentAppDatabase : DbContext
    {
        public shipmentAppDatabase(DbContextOptions options) : base(options) { }
        public DbSet<ShipmentModel> Shipments { get; set; }
        public DbSet<ShipmentStatusModel> ShipmentStatus { get; set; }
        public DbSet<CheckpointModel> ShipmentCheckpoints { get; set; }
        public DbSet<CheckpointsDistanceModel> CheckpointMappings { get; set; }
        public DbSet<RouteModel> Routes { get; set; }
		public DbSet<ShipmentPaymentMap> shipmentPaymentMaps { get; set; }
		public DbSet<RouteCheckpointsModel> RouteCheckpoints { get; set; }
    }
}
