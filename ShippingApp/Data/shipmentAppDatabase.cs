using Microsoft.EntityFrameworkCore;
using ShippingApp.Models;

namespace BlogApplication.Data
{
    public class shipmentAppDatabase : DbContext
    {
        public shipmentAppDatabase(DbContextOptions options) : base(options) { }
        /*protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<UserModel>().HasData(
                new UserModel
                {
                    firstName = "Admin",
                    lastName = "Admin",
                    email = "admin@gmail.com",
                    password = Encoding.ASCII.GetBytes("Admin@123"),
                    phoneNo = 9888636009,
                    isAdmin = true,
                });
        }*/
        // all tables in database
        public DbSet<ShipmentModel> Shipments { get; set; }
        public DbSet<ShipmentStatusModel> ShipmentStatus { get; set; }
    }
}
