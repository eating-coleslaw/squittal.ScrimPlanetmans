using Microsoft.EntityFrameworkCore.Migrations;

namespace squittal.ScrimPlanetmans.App.Migrations
{
    public partial class AdjustScrimVehDustModelTypesAndPK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ScrimVehicleDestruction",
                table: "ScrimVehicleDestruction");

            migrationBuilder.AlterColumn<int>(
                name: "Points",
                table: "ScrimVehicleDestruction",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "AttackerVehicleId",
                table: "ScrimVehicleDestruction",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScrimVehicleDestruction",
                table: "ScrimVehicleDestruction",
                columns: new[] { "ScrimMatchId", "Timestamp", "AttackerCharacterId", "VictimCharacterId", "VictimVehicleId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ScrimVehicleDestruction",
                table: "ScrimVehicleDestruction");

            migrationBuilder.AlterColumn<int>(
                name: "Points",
                table: "ScrimVehicleDestruction",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "AttackerVehicleId",
                table: "ScrimVehicleDestruction",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScrimVehicleDestruction",
                table: "ScrimVehicleDestruction",
                columns: new[] { "ScrimMatchId", "Timestamp", "AttackerCharacterId", "VictimCharacterId", "AttackerVehicleId", "VictimVehicleId" });
        }
    }
}
