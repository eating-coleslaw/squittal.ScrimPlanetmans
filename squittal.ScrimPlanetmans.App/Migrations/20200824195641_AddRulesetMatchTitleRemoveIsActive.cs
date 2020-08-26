using Microsoft.EntityFrameworkCore.Migrations;

namespace squittal.ScrimPlanetmans.App.Migrations
{
    public partial class AddRulesetMatchTitleRemoveIsActive : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Ruleset");

            migrationBuilder.AddColumn<string>(
                name: "DefaultMatchTitle",
                table: "Ruleset",
                nullable: true,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultMatchTitle",
                table: "Ruleset");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Ruleset",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
