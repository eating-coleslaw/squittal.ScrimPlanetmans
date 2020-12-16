using Microsoft.EntityFrameworkCore.Migrations;

namespace squittal.ScrimPlanetmans.App.Migrations
{
    public partial class AddOverlayRulesetSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OverlayStatsDisplayType",
                table: "Ruleset",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<bool>(
                name: "UseCompactOverlay",
                table: "Ruleset",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OverlayStatsDisplayType",
                table: "Ruleset");

            migrationBuilder.DropColumn(
                name: "UseCompactOverlay",
                table: "Ruleset");
        }
    }
}
