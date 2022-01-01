using Microsoft.EntityFrameworkCore.Migrations;

namespace squittal.ScrimPlanetmans.App.Migrations
{
    public partial class InitialConquestRulesetUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EnablePeriodicFacilityControlRewards",
                table: "Ruleset",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EnableRoundTimeLimit",
                table: "Ruleset",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EndRoundOnPointValueReached",
                table: "Ruleset",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "InitialPoints",
                table: "Ruleset",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MatchWinCondition",
                table: "Ruleset",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PeriodFacilityControlPointAttributionType",
                table: "Ruleset",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PeriodicFacilityControlInterval",
                table: "Ruleset",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PeriodicFacilityControlPoints",
                table: "Ruleset",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoundTimerDirection",
                table: "Ruleset",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoundWinCondition",
                table: "Ruleset",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TargetPointValue",
                table: "Ruleset",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnablePeriodicFacilityControlRewards",
                table: "Ruleset");

            migrationBuilder.DropColumn(
                name: "EnableRoundTimeLimit",
                table: "Ruleset");

            migrationBuilder.DropColumn(
                name: "EndRoundOnPointValueReached",
                table: "Ruleset");

            migrationBuilder.DropColumn(
                name: "InitialPoints",
                table: "Ruleset");

            migrationBuilder.DropColumn(
                name: "MatchWinCondition",
                table: "Ruleset");

            migrationBuilder.DropColumn(
                name: "PeriodFacilityControlPointAttributionType",
                table: "Ruleset");

            migrationBuilder.DropColumn(
                name: "PeriodicFacilityControlInterval",
                table: "Ruleset");

            migrationBuilder.DropColumn(
                name: "PeriodicFacilityControlPoints",
                table: "Ruleset");

            migrationBuilder.DropColumn(
                name: "RoundTimerDirection",
                table: "Ruleset");

            migrationBuilder.DropColumn(
                name: "RoundWinCondition",
                table: "Ruleset");

            migrationBuilder.DropColumn(
                name: "TargetPointValue",
                table: "Ruleset");
        }
    }
}
