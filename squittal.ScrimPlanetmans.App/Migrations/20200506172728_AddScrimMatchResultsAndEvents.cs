using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace squittal.ScrimPlanetmans.App.Migrations
{
    public partial class AddScrimMatchResultsAndEvents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScrimDeath",
                columns: table => new
                {
                    ScrimMatchId = table.Column<string>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    AttackerCharacterId = table.Column<string>(nullable: false),
                    VictimCharacterId = table.Column<string>(nullable: false),
                    ScrimMatchRound = table.Column<int>(nullable: false, defaultValue: -1),
                    ActionType = table.Column<int>(nullable: false),
                    DeathType = table.Column<int>(nullable: false),
                    AttackerTeamOrdinal = table.Column<int>(nullable: false),
                    VictimTeamOrdinal = table.Column<int>(nullable: false),
                    AttackerNameFull = table.Column<string>(nullable: true),
                    AttackerFactionId = table.Column<int>(nullable: false),
                    AttackerLoadoutId = table.Column<int>(nullable: true),
                    AttackerOutfitId = table.Column<string>(nullable: true),
                    AttackerOutfitAlias = table.Column<string>(nullable: true),
                    AttackerIsOutfitless = table.Column<bool>(nullable: false),
                    VictimNameFull = table.Column<string>(nullable: true),
                    VictimFactionId = table.Column<int>(nullable: false),
                    VictimLoadoutId = table.Column<int>(nullable: true),
                    VictimOutfitId = table.Column<string>(nullable: true),
                    VictimOutfitAlias = table.Column<string>(nullable: true),
                    VictimIsOutfitless = table.Column<bool>(nullable: false),
                    IsHeadshot = table.Column<bool>(nullable: false),
                    WeaponId = table.Column<int>(nullable: true),
                    WeaponItemCategoryId = table.Column<int>(nullable: true),
                    IsVehicleWeapon = table.Column<bool>(nullable: true),
                    AttackerVehicleId = table.Column<int>(nullable: true),
                    WorldId = table.Column<int>(nullable: false),
                    ZoneId = table.Column<int>(nullable: false),
                    Points = table.Column<int>(nullable: false),
                    AttackerResultingPoints = table.Column<int>(nullable: true),
                    AttackerResultingNetScore = table.Column<int>(nullable: true),
                    VictimResultingPoints = table.Column<int>(nullable: true),
                    VictimResultingNetScore = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScrimDeath", x => new { x.ScrimMatchId, x.Timestamp, x.AttackerCharacterId, x.VictimCharacterId });
                });

            migrationBuilder.CreateTable(
                name: "ScrimMatch",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    StartTime = table.Column<DateTime>(nullable: false),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScrimMatch", x => x.Id);
                });

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
                name: "ScrimVehicleDestruction",
                columns: table => new
                {
                    ScrimMatchId = table.Column<string>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    AttackerCharacterId = table.Column<string>(nullable: false),
                    VictimCharacterId = table.Column<string>(nullable: false),
                    VictimVehicleId = table.Column<int>(nullable: false),
                    AttackerVehicleId = table.Column<int>(nullable: true),
                    ScrimMatchRound = table.Column<int>(nullable: false, defaultValue: -1),
                    ActionType = table.Column<int>(nullable: false),
                    DeathType = table.Column<int>(nullable: false),
                    AttackerTeamOrdinal = table.Column<int>(nullable: false),
                    VictimTeamOrdinal = table.Column<int>(nullable: false),
                    AttackerVehicleType = table.Column<int>(nullable: true),
                    VictimVehicleType = table.Column<int>(nullable: true),
                    AttackerNameFull = table.Column<string>(nullable: true),
                    AttackerFactionId = table.Column<int>(nullable: false),
                    AttackerLoadoutId = table.Column<int>(nullable: true),
                    AttackerOutfitId = table.Column<string>(nullable: true),
                    AttackerOutfitAlias = table.Column<string>(nullable: true),
                    AttackerIsOutfitless = table.Column<bool>(nullable: false),
                    VictimNameFull = table.Column<string>(nullable: true),
                    VictimFactionId = table.Column<int>(nullable: false),
                    VictimLoadoutId = table.Column<int>(nullable: true),
                    VictimOutfitId = table.Column<string>(nullable: true),
                    VictimOutfitAlias = table.Column<string>(nullable: true),
                    VictimIsOutfitless = table.Column<bool>(nullable: false),
                    WeaponId = table.Column<int>(nullable: true),
                    WeaponItemCategoryId = table.Column<int>(nullable: true),
                    IsVehicleWeapon = table.Column<bool>(nullable: true),
                    WorldId = table.Column<int>(nullable: false),
                    ZoneId = table.Column<int>(nullable: false),
                    Points = table.Column<int>(nullable: false, defaultValue: 0),
                    AttackerResultingPoints = table.Column<int>(nullable: true),
                    AttackerResultingNetScore = table.Column<int>(nullable: true),
                    VictimResultingPoints = table.Column<int>(nullable: true),
                    VictimResultingNetScore = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScrimVehicleDestruction", x => new { x.ScrimMatchId, x.Timestamp, x.AttackerCharacterId, x.VictimCharacterId, x.VictimVehicleId });
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
                name: "ScrimDeath");

            migrationBuilder.DropTable(
                name: "ScrimMatch");

            migrationBuilder.DropTable(
                name: "ScrimMatchTeamPointAdjustment");

            migrationBuilder.DropTable(
                name: "ScrimVehicleDestruction");

            migrationBuilder.DropTable(
                name: "ScrimMatchTeamResult");
        }
    }
}
