using Microsoft.EntityFrameworkCore.Migrations;

namespace squittal.ScrimPlanetmans.App.Migrations
{
    public partial class AddMatchRoundColumnsToEventTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ScrimMatchRound",
                table: "ScrimVehicleDestruction",
                nullable: false,
                defaultValue: -1);

            migrationBuilder.AddColumn<int>(
                name: "ScrimMatchRound",
                table: "ScrimDeath",
                nullable: false,
                defaultValue: -1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScrimMatchRound",
                table: "ScrimVehicleDestruction");

            migrationBuilder.DropColumn(
                name: "ScrimMatchRound",
                table: "ScrimDeath");
        }
    }
}
