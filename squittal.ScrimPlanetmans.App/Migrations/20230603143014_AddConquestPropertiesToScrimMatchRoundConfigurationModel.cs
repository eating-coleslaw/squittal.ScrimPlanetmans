using Microsoft.EntityFrameworkCore.Migrations;

namespace squittal.ScrimPlanetmans.App.Migrations
{
    public partial class AddConquestPropertiesToScrimMatchRoundConfigurationModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EnablePeriodicFacilityControlRewards",
                table: "ScrimMatchRoundConfiguration",
                nullable: true,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EnableRoundTimeLimit",
                table: "ScrimMatchRoundConfiguration",
                nullable: true,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "EndRoundOnPointValueReached",
                table: "ScrimMatchRoundConfiguration",
                nullable: true,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "InitialPoints",
                table: "ScrimMatchRoundConfiguration",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MatchWinCondition",
                table: "ScrimMatchRoundConfiguration",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PeriodicFacilityControlInterval",
                table: "ScrimMatchRoundConfiguration",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PeriodicFacilityControlPoints",
                table: "ScrimMatchRoundConfiguration",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoundTimerDirection",
                table: "ScrimMatchRoundConfiguration",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoundWinCondition",
                table: "ScrimMatchRoundConfiguration",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TargetPointValue",
                table: "ScrimMatchRoundConfiguration",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnablePeriodicFacilityControlRewards",
                table: "ScrimMatchRoundConfiguration");

            migrationBuilder.DropColumn(
                name: "EnableRoundTimeLimit",
                table: "ScrimMatchRoundConfiguration");

            migrationBuilder.DropColumn(
                name: "EndRoundOnPointValueReached",
                table: "ScrimMatchRoundConfiguration");

            migrationBuilder.DropColumn(
                name: "InitialPoints",
                table: "ScrimMatchRoundConfiguration");

            migrationBuilder.DropColumn(
                name: "MatchWinCondition",
                table: "ScrimMatchRoundConfiguration");

            migrationBuilder.DropColumn(
                name: "PeriodicFacilityControlInterval",
                table: "ScrimMatchRoundConfiguration");

            migrationBuilder.DropColumn(
                name: "PeriodicFacilityControlPoints",
                table: "ScrimMatchRoundConfiguration");

            migrationBuilder.DropColumn(
                name: "RoundTimerDirection",
                table: "ScrimMatchRoundConfiguration");

            migrationBuilder.DropColumn(
                name: "RoundWinCondition",
                table: "ScrimMatchRoundConfiguration");

            migrationBuilder.DropColumn(
                name: "TargetPointValue",
                table: "ScrimMatchRoundConfiguration");
        }
    }
}
