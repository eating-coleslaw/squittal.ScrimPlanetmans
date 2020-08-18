using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.IO;

namespace squittal.ScrimPlanetmans.App.Migrations
{
    public partial class AddRulesetIdToScrimMatchModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RulesetId",
                table: "ScrimMatch",
                nullable: false,
                defaultValue: -1);

            migrationBuilder.CreateIndex(
                name: "IX_ScrimMatch_RulesetId",
                table: "ScrimMatch",
                column: "RulesetId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ScrimMatch_Ruleset_RulesetId",
                table: "ScrimMatch",
                column: "RulesetId",
                principalTable: "Ruleset",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            var sqlFile = "Data/SQL/MigrationHelpers/Backfill_ScrimMatchRulesetId.sql";
            var basePath = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
            var filePath = Path.Combine(basePath, sqlFile);
            migrationBuilder.Sql(File.ReadAllText(filePath));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScrimMatch_Ruleset_RulesetId",
                table: "ScrimMatch");

            migrationBuilder.DropIndex(
                name: "IX_ScrimMatch_RulesetId",
                table: "ScrimMatch");

            migrationBuilder.DropColumn(
                name: "RulesetId",
                table: "ScrimMatch");
        }
    }
}
