using Microsoft.EntityFrameworkCore.Migrations;

namespace squittal.ScrimPlanetmans.App.Migrations
{
    public partial class AddPlanetsideClassSettingsToItemItemCatRuleModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DeferToPlanetsideClassSettings",
                table: "RulesetItemRule",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EngineerIsBanned",
                table: "RulesetItemRule",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "EngineerPoints",
                table: "RulesetItemRule",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "HeavyAssaultIsBanned",
                table: "RulesetItemRule",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "HeavyAssaultPoints",
                table: "RulesetItemRule",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "InfiltratorIsBanned",
                table: "RulesetItemRule",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "InfiltratorPoints",
                table: "RulesetItemRule",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "LightAssaultIsBanned",
                table: "RulesetItemRule",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "LightAssaultPoints",
                table: "RulesetItemRule",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "MaxIsBanned",
                table: "RulesetItemRule",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MaxPoints",
                table: "RulesetItemRule",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "MedicIsBanned",
                table: "RulesetItemRule",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MedicPoints",
                table: "RulesetItemRule",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "DeferToPlanetsideClassSettings",
                table: "RulesetItemCategoryRule",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EngineerIsBanned",
                table: "RulesetItemCategoryRule",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "EngineerPoints",
                table: "RulesetItemCategoryRule",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "HeavyAssaultIsBanned",
                table: "RulesetItemCategoryRule",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "HeavyAssaultPoints",
                table: "RulesetItemCategoryRule",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "InfiltratorIsBanned",
                table: "RulesetItemCategoryRule",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "InfiltratorPoints",
                table: "RulesetItemCategoryRule",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "LightAssaultIsBanned",
                table: "RulesetItemCategoryRule",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "LightAssaultPoints",
                table: "RulesetItemCategoryRule",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "MaxIsBanned",
                table: "RulesetItemCategoryRule",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MaxPoints",
                table: "RulesetItemCategoryRule",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "MedicIsBanned",
                table: "RulesetItemCategoryRule",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MedicPoints",
                table: "RulesetItemCategoryRule",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeferToPlanetsideClassSettings",
                table: "RulesetItemRule");

            migrationBuilder.DropColumn(
                name: "EngineerIsBanned",
                table: "RulesetItemRule");

            migrationBuilder.DropColumn(
                name: "EngineerPoints",
                table: "RulesetItemRule");

            migrationBuilder.DropColumn(
                name: "HeavyAssaultIsBanned",
                table: "RulesetItemRule");

            migrationBuilder.DropColumn(
                name: "HeavyAssaultPoints",
                table: "RulesetItemRule");

            migrationBuilder.DropColumn(
                name: "InfiltratorIsBanned",
                table: "RulesetItemRule");

            migrationBuilder.DropColumn(
                name: "InfiltratorPoints",
                table: "RulesetItemRule");

            migrationBuilder.DropColumn(
                name: "LightAssaultIsBanned",
                table: "RulesetItemRule");

            migrationBuilder.DropColumn(
                name: "LightAssaultPoints",
                table: "RulesetItemRule");

            migrationBuilder.DropColumn(
                name: "MaxIsBanned",
                table: "RulesetItemRule");

            migrationBuilder.DropColumn(
                name: "MaxPoints",
                table: "RulesetItemRule");

            migrationBuilder.DropColumn(
                name: "MedicIsBanned",
                table: "RulesetItemRule");

            migrationBuilder.DropColumn(
                name: "MedicPoints",
                table: "RulesetItemRule");

            migrationBuilder.DropColumn(
                name: "DeferToPlanetsideClassSettings",
                table: "RulesetItemCategoryRule");

            migrationBuilder.DropColumn(
                name: "EngineerIsBanned",
                table: "RulesetItemCategoryRule");

            migrationBuilder.DropColumn(
                name: "EngineerPoints",
                table: "RulesetItemCategoryRule");

            migrationBuilder.DropColumn(
                name: "HeavyAssaultIsBanned",
                table: "RulesetItemCategoryRule");

            migrationBuilder.DropColumn(
                name: "HeavyAssaultPoints",
                table: "RulesetItemCategoryRule");

            migrationBuilder.DropColumn(
                name: "InfiltratorIsBanned",
                table: "RulesetItemCategoryRule");

            migrationBuilder.DropColumn(
                name: "InfiltratorPoints",
                table: "RulesetItemCategoryRule");

            migrationBuilder.DropColumn(
                name: "LightAssaultIsBanned",
                table: "RulesetItemCategoryRule");

            migrationBuilder.DropColumn(
                name: "LightAssaultPoints",
                table: "RulesetItemCategoryRule");

            migrationBuilder.DropColumn(
                name: "MaxIsBanned",
                table: "RulesetItemCategoryRule");

            migrationBuilder.DropColumn(
                name: "MaxPoints",
                table: "RulesetItemCategoryRule");

            migrationBuilder.DropColumn(
                name: "MedicIsBanned",
                table: "RulesetItemCategoryRule");

            migrationBuilder.DropColumn(
                name: "MedicPoints",
                table: "RulesetItemCategoryRule");
        }
    }
}
