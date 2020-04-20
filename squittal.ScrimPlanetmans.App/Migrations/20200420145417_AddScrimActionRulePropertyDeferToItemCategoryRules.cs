using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.IO;

namespace squittal.ScrimPlanetmans.App.Migrations
{
    public partial class AddScrimActionRulePropertyDeferToItemCategoryRules : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DeferToItemCategoryRules",
                table: "RulesetActionRule",
                nullable: false,
                defaultValue: false);

            var sqlFile = "Data/SQL/MigrationHelpers/Backfill_RulesetActionRuleDeferToItemCategoryRules.sql";
            var basePath = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
            var filePath = Path.Combine(basePath, sqlFile);
            migrationBuilder.Sql(File.ReadAllText(filePath));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeferToItemCategoryRules",
                table: "RulesetActionRule");
        }
    }
}
