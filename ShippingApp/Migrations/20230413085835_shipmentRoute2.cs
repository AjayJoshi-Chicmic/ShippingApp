using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShippingApp.Migrations
{
    /// <inheritdoc />
    public partial class shipmentRoute2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Checkpoints",
                table: "Checkpoints");

            migrationBuilder.RenameTable(
                name: "Checkpoints",
                newName: "Checkpoint");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Checkpoint",
                table: "Checkpoint",
                column: "checkpointId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Checkpoint",
                table: "Checkpoint");

            migrationBuilder.RenameTable(
                name: "Checkpoint",
                newName: "Checkpoints");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Checkpoints",
                table: "Checkpoints",
                column: "checkpointId");
        }
    }
}
