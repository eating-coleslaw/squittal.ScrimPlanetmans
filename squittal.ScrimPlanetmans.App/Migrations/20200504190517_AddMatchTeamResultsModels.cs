using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace squittal.ScrimPlanetmans.App.Migrations
{
    public partial class AddMatchTeamResultsModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScrimMatchTeamResult",
                columns: table => new
                {
                    ScrimMatchId = table.Column<string>(nullable: false),
                    TeamOrdinal = table.Column<int>(nullable: false),
                    Points = table.Column<int>(nullable: false, defaultValue: 0),
                    NetScore = table.Column<int>(nullable: false, defaultValue: 0),
                    Kills = table.Column<int>(nullable: false, defaultValue: 0),
                    Deaths = table.Column<int>(nullable: false, defaultValue: 0),
                    Headshots = table.Column<int>(nullable: false, defaultValue: 0),
                    HeadshotDeaths = table.Column<int>(nullable: false, defaultValue: 0),
                    Suicides = table.Column<int>(nullable: false, defaultValue: 0),
                    Teamkills = table.Column<int>(nullable: false, defaultValue: 0),
                    TeamkillDeaths = table.Column<int>(nullable: false, defaultValue: 0),
                    RevivesGiven = table.Column<int>(nullable: false, defaultValue: 0),
                    RevivesTaken = table.Column<int>(nullable: false, defaultValue: 0),
                    DamageAssists = table.Column<int>(nullable: false, defaultValue: 0),
                    UtilityAssists = table.Column<int>(nullable: false, defaultValue: 0),
                    DamageAssistedDeaths = table.Column<int>(nullable: false, defaultValue: 0),
                    UtilityAssistedDeaths = table.Column<int>(nullable: false, defaultValue: 0),
                    ObjectiveCaptureTicks = table.Column<int>(nullable: false, defaultValue: 0),
                    ObjectiveDefenseTicks = table.Column<int>(nullable: false, defaultValue: 0),
                    BaseDefenses = table.Column<int>(nullable: false, defaultValue: 0),
                    BaseCaptures = table.Column<int>(nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScrimMatchTeamResult", x => new { x.ScrimMatchId, x.TeamOrdinal });
                });

            migrationBuilder.CreateTable(
                name: "ScrimMatchTeamPointAdjustment",
                columns: table => new
                {
                    ScrimMatchId = table.Column<string>(nullable: false),
                    TeamOrdinal = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    Points = table.Column<int>(nullable: false, defaultValue: 0),
                    AdjustmentType = table.Column<int>(nullable: false),
                    Rationale = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScrimMatchTeamPointAdjustment", x => new { x.ScrimMatchId, x.TeamOrdinal });
                    table.ForeignKey(
                        name: "FK_ScrimMatchTeamPointAdjustment_ScrimMatchTeamResult_ScrimMatchId_TeamOrdinal",
                        columns: x => new { x.ScrimMatchId, x.TeamOrdinal },
                        principalTable: "ScrimMatchTeamResult",
                        principalColumns: new[] { "ScrimMatchId", "TeamOrdinal" },
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScrimMatchTeamPointAdjustment");

            migrationBuilder.DropTable(
                name: "ScrimMatchTeamResult");
        }
    }
}
