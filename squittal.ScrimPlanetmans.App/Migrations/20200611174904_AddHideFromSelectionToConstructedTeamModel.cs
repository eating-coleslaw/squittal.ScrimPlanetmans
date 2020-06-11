using Microsoft.EntityFrameworkCore.Migrations;

namespace squittal.ScrimPlanetmans.App.Migrations
{
    public partial class AddHideFromSelectionToConstructedTeamModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HideFromSelection",
                table: "ConstructedTeam",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HideFromSelection",
                table: "ConstructedTeam");
        }
    }
}
