using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace squittal.ScrimPlanetmans.App.Migrations
{
    public partial class AddScrimMatchDeathVehDestructionModels : Migration
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
                    AttackerResultingPoints = table.Column<int>(nullable: false),
                    AttackerResultingNetScore = table.Column<int>(nullable: false),
                    VictimResultingPoints = table.Column<int>(nullable: false),
                    VictimResultingNetScore = table.Column<int>(nullable: false)
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
                name: "ScrimVehicleDestruction",
                columns: table => new
                {
                    ScrimMatchId = table.Column<string>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    AttackerCharacterId = table.Column<string>(nullable: false),
                    AttackerVehicleId = table.Column<int>(nullable: false),
                    VictimCharacterId = table.Column<string>(nullable: false),
                    VictimVehicleId = table.Column<int>(nullable: false),
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
                    Points = table.Column<int>(nullable: false),
                    AttackerResultingPoints = table.Column<int>(nullable: false),
                    AttackerResultingNetScore = table.Column<int>(nullable: false),
                    VictimResultingPoints = table.Column<int>(nullable: false),
                    VictimResultingNetScore = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScrimVehicleDestruction", x => new { x.ScrimMatchId, x.Timestamp, x.AttackerCharacterId, x.VictimCharacterId, x.AttackerVehicleId, x.VictimVehicleId });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScrimDeath");

            migrationBuilder.DropTable(
                name: "ScrimMatch");

            migrationBuilder.DropTable(
                name: "ScrimVehicleDestruction");
        }
    }
}
