using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShippingApp.Migrations
{
    /// <inheritdoc />
    public partial class @new : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Shipments",
                columns: table => new
                {
                    shipmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    customerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    productTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    cointainerTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    shipmentPrice = table.Column<float>(type: "real", nullable: false),
                    shipmentWeight = table.Column<float>(type: "real", nullable: false),
                    origin = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    destination = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    dateOfOrder = table.Column<DateTime>(type: "datetime2", nullable: false),
                    shipmentStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isDeleted = table.Column<bool>(type: "bit", nullable: false),
                    lastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipments", x => x.shipmentId);
                });

            migrationBuilder.CreateTable(
                name: "ShipmentStatus",
                columns: table => new
                {
                    shipmentStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    shipmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    shipmentStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    currentLocation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    lastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentStatus", x => x.shipmentStatusId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Shipments");

            migrationBuilder.DropTable(
                name: "ShipmentStatus");
        }
    }
}
