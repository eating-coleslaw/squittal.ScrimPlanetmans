using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace squittal.ScrimPlanetmans.App.Migrations
{
    public partial class AddPeriodicControlTickModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScrimPeriodicControlTick",
                columns: table => new
                {
                    ScrimMatchId = table.Column<string>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    ScrimMatchRound = table.Column<int>(nullable: false),
                    TeamOrdinal = table.Column<int>(nullable: false),
                    Points = table.Column<int>(nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScrimPeriodicControlTick", x => new { x.ScrimMatchId, x.Timestamp });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScrimPeriodicControlTick");
        }
    }
}
