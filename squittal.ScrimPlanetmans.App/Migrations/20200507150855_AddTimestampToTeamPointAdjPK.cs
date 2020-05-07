using Microsoft.EntityFrameworkCore.Migrations;

namespace squittal.ScrimPlanetmans.App.Migrations
{
    public partial class AddTimestampToTeamPointAdjPK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ScrimMatchTeamPointAdjustment",
                table: "ScrimMatchTeamPointAdjustment");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScrimMatchTeamPointAdjustment",
                table: "ScrimMatchTeamPointAdjustment",
                columns: new[] { "ScrimMatchId", "TeamOrdinal", "Timestamp" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ScrimMatchTeamPointAdjustment",
                table: "ScrimMatchTeamPointAdjustment");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScrimMatchTeamPointAdjustment",
                table: "ScrimMatchTeamPointAdjustment",
                columns: new[] { "ScrimMatchId", "TeamOrdinal" });
        }
    }
}
