using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShippingApp.Migrations
{
    /// <inheritdoc />
    public partial class shipmentRoute3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Checkpoint",
                table: "Checkpoint");

            migrationBuilder.RenameTable(
                name: "Checkpoint",
                newName: "ShipmentCheckpoints");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShipmentCheckpoints",
                table: "ShipmentCheckpoints",
                column: "checkpointId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ShipmentCheckpoints",
                table: "ShipmentCheckpoints");

            migrationBuilder.RenameTable(
                name: "ShipmentCheckpoints",
                newName: "Checkpoint");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Checkpoint",
                table: "Checkpoint",
                column: "checkpointId");
        }
    }
}
