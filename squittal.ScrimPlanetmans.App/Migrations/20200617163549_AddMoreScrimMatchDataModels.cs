using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace squittal.ScrimPlanetmans.App.Migrations
{
    public partial class AddMoreScrimMatchDataModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScrimDamageAssist",
                columns: table => new
                {
                    ScrimMatchId = table.Column<string>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    AttackerCharacterId = table.Column<string>(nullable: false),
                    VictimCharacterId = table.Column<string>(nullable: false),
                    ScrimMatchRound = table.Column<int>(nullable: false),
                    ActionType = table.Column<int>(nullable: false),
                    AttackerTeamOrdinal = table.Column<int>(nullable: false),
                    VictimTeamOrdinal = table.Column<int>(nullable: false),
                    AttackerLoadoutId = table.Column<int>(nullable: true),
                    VictimLoadoutId = table.Column<int>(nullable: true),
                    ExperienceGainId = table.Column<int>(nullable: false),
                    ExperienceGainAmount = table.Column<int>(nullable: false, defaultValue: 0),
                    ZoneId = table.Column<int>(nullable: true),
                    WorldId = table.Column<int>(nullable: false),
                    Points = table.Column<int>(nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScrimDamageAssist", x => new { x.ScrimMatchId, x.Timestamp, x.AttackerCharacterId, x.VictimCharacterId });
                });

            migrationBuilder.CreateTable(
                name: "ScrimFacilityControl",
                columns: table => new
                {
                    ScrimMatchId = table.Column<string>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    FacilityId = table.Column<int>(nullable: false),
                    ControllingTeamOrdinal = table.Column<int>(nullable: false),
                    ScrimMatchRound = table.Column<int>(nullable: false),
                    ControlType = table.Column<int>(nullable: false),
                    ControllingFactionId = table.Column<int>(nullable: false),
                    ZoneId = table.Column<int>(nullable: true),
                    WorldId = table.Column<int>(nullable: false),
                    Points = table.Column<int>(nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScrimFacilityControl", x => new { x.ScrimMatchId, x.Timestamp, x.FacilityId, x.ControllingTeamOrdinal });
                });

            migrationBuilder.CreateTable(
                name: "ScrimGrenadeAssist",
                columns: table => new
                {
                    ScrimMatchId = table.Column<string>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    AttackerCharacterId = table.Column<string>(nullable: false),
                    VictimCharacterId = table.Column<string>(nullable: false),
                    ScrimMatchRound = table.Column<int>(nullable: false),
                    ActionType = table.Column<int>(nullable: false),
                    AttackerTeamOrdinal = table.Column<int>(nullable: false),
                    VictimTeamOrdinal = table.Column<int>(nullable: false),
                    AttackerLoadoutId = table.Column<int>(nullable: true),
                    VictimLoadoutId = table.Column<int>(nullable: true),
                    ExperienceGainId = table.Column<int>(nullable: false),
                    ExperienceGainAmount = table.Column<int>(nullable: false, defaultValue: 0),
                    ZoneId = table.Column<int>(nullable: true),
                    WorldId = table.Column<int>(nullable: false),
                    Points = table.Column<int>(nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScrimGrenadeAssist", x => new { x.ScrimMatchId, x.Timestamp, x.AttackerCharacterId, x.VictimCharacterId });
                });

            migrationBuilder.CreateTable(
                name: "ScrimMatchParticipatingPlayer",
                columns: table => new
                {
                    ScrimMatchId = table.Column<string>(nullable: false),
                    CharacterId = table.Column<string>(nullable: false),
                    TeamOrdinal = table.Column<int>(nullable: false),
                    NameFull = table.Column<string>(nullable: true),
                    NameDisplay = table.Column<string>(nullable: true),
                    FactionId = table.Column<int>(nullable: false),
                    WorldId = table.Column<int>(nullable: false),
                    PrestigeLevel = table.Column<int>(nullable: false),
                    IsFromOutfit = table.Column<bool>(nullable: false, defaultValue: false),
                    OutfitId = table.Column<string>(nullable: true),
                    OutfitAlias = table.Column<string>(nullable: true),
                    IsFromConstructedTeam = table.Column<bool>(nullable: false, defaultValue: false),
                    ConstructedTeamId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScrimMatchParticipatingPlayer", x => new { x.ScrimMatchId, x.CharacterId });
                });

            migrationBuilder.CreateTable(
                name: "ScrimMatchRoundConfiguration",
                columns: table => new
                {
                    ScrimMatchId = table.Column<string>(nullable: false),
                    ScrimMatchRound = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    RoundSecondsTotal = table.Column<int>(nullable: false),
                    WorldId = table.Column<int>(nullable: false),
                    IsManualWorldId = table.Column<bool>(nullable: false, defaultValue: false),
                    FacilityId = table.Column<int>(nullable: true),
                    IsRoundEndedOnFacilityCapture = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScrimMatchRoundConfiguration", x => new { x.ScrimMatchId, x.ScrimMatchRound });
                });

            migrationBuilder.CreateTable(
                name: "ScrimRevive",
                columns: table => new
                {
                    ScrimMatchId = table.Column<string>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    MedicCharacterId = table.Column<string>(nullable: false),
                    RevivedCharacterId = table.Column<string>(nullable: false),
                    ScrimMatchRound = table.Column<int>(nullable: false),
                    ActionType = table.Column<int>(nullable: false),
                    MedicTeamOrdinal = table.Column<int>(nullable: false),
                    RevivedTeamOrdinal = table.Column<int>(nullable: false),
                    MedicLoadoutId = table.Column<int>(nullable: true),
                    RevivedLoadoutId = table.Column<int>(nullable: true),
                    ExperienceGainId = table.Column<int>(nullable: false),
                    ExperienceGainAmount = table.Column<int>(nullable: false, defaultValue: 0),
                    ZoneId = table.Column<int>(nullable: true),
                    WorldId = table.Column<int>(nullable: false),
                    Points = table.Column<int>(nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScrimRevive", x => new { x.ScrimMatchId, x.Timestamp, x.MedicCharacterId, x.RevivedCharacterId });
                });

            migrationBuilder.CreateTable(
                name: "ScrimSpotAssist",
                columns: table => new
                {
                    ScrimMatchId = table.Column<string>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    SpotterCharacterId = table.Column<string>(nullable: false),
                    VictimCharacterId = table.Column<string>(nullable: false),
                    ScrimMatchRound = table.Column<int>(nullable: false),
                    ActionType = table.Column<int>(nullable: false),
                    SpotterTeamOrdinal = table.Column<int>(nullable: false),
                    VictimTeamOrdinal = table.Column<int>(nullable: false),
                    SpotterLoadoutId = table.Column<int>(nullable: true),
                    VictimLoadoutId = table.Column<int>(nullable: true),
                    ExperienceGainId = table.Column<int>(nullable: false),
                    ExperienceGainAmount = table.Column<int>(nullable: false, defaultValue: 0),
                    ZoneId = table.Column<int>(nullable: true),
                    WorldId = table.Column<int>(nullable: false),
                    Points = table.Column<int>(nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScrimSpotAssist", x => new { x.ScrimMatchId, x.Timestamp, x.SpotterCharacterId, x.VictimCharacterId });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScrimDamageAssist");

            migrationBuilder.DropTable(
                name: "ScrimFacilityControl");

            migrationBuilder.DropTable(
                name: "ScrimGrenadeAssist");

            migrationBuilder.DropTable(
                name: "ScrimMatchParticipatingPlayer");

            migrationBuilder.DropTable(
                name: "ScrimMatchRoundConfiguration");

            migrationBuilder.DropTable(
                name: "ScrimRevive");

            migrationBuilder.DropTable(
                name: "ScrimSpotAssist");
        }
    }
}
