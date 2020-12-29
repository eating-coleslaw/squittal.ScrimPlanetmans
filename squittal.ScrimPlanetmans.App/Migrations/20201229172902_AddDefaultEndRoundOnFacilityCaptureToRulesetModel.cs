using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.IO;

namespace squittal.ScrimPlanetmans.App.Migrations
{
    public partial class AddDefaultEndRoundOnFacilityCaptureToRulesetModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DefaultMatchTitle",
                table: "Ruleset",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldDefaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "DefaultEndRoundOnFacilityCapture",
                table: "Ruleset",
                nullable: false,
                defaultValue: false);

            var sqlFile = "Data/SQL/MigrationHelpers/Backfill_RulesetSourceFileNulls.sql";
            var basePath = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
            var filePath = Path.Combine(basePath, sqlFile);
            migrationBuilder.Sql(File.ReadAllText(filePath));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultEndRoundOnFacilityCapture",
                table: "Ruleset");

            migrationBuilder.AlterColumn<string>(
                name: "DefaultMatchTitle",
                table: "Ruleset",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "",
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
