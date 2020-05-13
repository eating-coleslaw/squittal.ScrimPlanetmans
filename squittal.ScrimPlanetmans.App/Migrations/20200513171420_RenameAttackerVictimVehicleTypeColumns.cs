using Microsoft.EntityFrameworkCore.Migrations;

namespace squittal.ScrimPlanetmans.App.Migrations
{
    public partial class RenameAttackerVictimVehicleTypeColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AttackerVehicleType",
                table: "ScrimVehicleDestruction",
                newName: "AttackerVehicleClass");

            migrationBuilder.RenameColumn(
                name: "VictimVehicleType",
                table: "ScrimVehicleDestruction",
                newName: "VictimVehicleClass");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AttackerVehicleClass",
                table: "ScrimVehicleDestruction",
                newName: "AttackerVehicleType");

            migrationBuilder.RenameColumn(
                name: "VictimVehicleClass",
                table: "ScrimVehicleDestruction",
                newName: "VictimVehicleType");
        }
    }
}
