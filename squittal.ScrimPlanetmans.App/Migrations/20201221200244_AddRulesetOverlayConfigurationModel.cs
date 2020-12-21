using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.IO;

namespace squittal.ScrimPlanetmans.App.Migrations
{
    public partial class AddRulesetOverlayConfigurationModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RulesetOverlayConfiguration",
                columns: table => new
                {
                    RulesetId = table.Column<int>(nullable: false),
                    UseCompactLayout = table.Column<bool>(nullable: false, defaultValue: false),
                    StatsDisplayType = table.Column<int>(nullable: false, defaultValue: 1),
                    ShowStatusPanelScores = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RulesetOverlayConfiguration", x => x.RulesetId);
                    table.ForeignKey(
                        name: "FK_RulesetOverlayConfiguration_Ruleset_RulesetId",
                        column: x => x.RulesetId,
                        principalTable: "Ruleset",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            var sqlFile = "Data/SQL/MigrationHelpers/Backfill_RulesetOverlayConfiguration.sql";
            var basePath = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
            var filePath = Path.Combine(basePath, sqlFile);
            migrationBuilder.Sql(File.ReadAllText(filePath));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RulesetOverlayConfiguration");
        }
    }
}
