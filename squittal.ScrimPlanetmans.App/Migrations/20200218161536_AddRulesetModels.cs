using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace squittal.ScrimPlanetmans.App.Migrations
{
    public partial class AddRulesetModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ruleset",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateLastModified = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ruleset", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScrimAction",
                columns: table => new
                {
                    Action = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScrimAction", x => x.Action);
                });

            migrationBuilder.CreateTable(
                name: "RulesetActionRule",
                columns: table => new
                {
                    RulesetId = table.Column<int>(nullable: false),
                    ScrimActionType = table.Column<int>(nullable: false),
                    Points = table.Column<int>(nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RulesetActionRule", x => new { x.RulesetId, x.ScrimActionType });
                    table.ForeignKey(
                        name: "FK_RulesetActionRule_Ruleset_RulesetId",
                        column: x => x.RulesetId,
                        principalTable: "Ruleset",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RulesetItemCategoryRule",
                columns: table => new
                {
                    RulesetId = table.Column<int>(nullable: false),
                    ItemCategoryId = table.Column<int>(nullable: false),
                    Points = table.Column<int>(nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RulesetItemCategoryRule", x => new { x.RulesetId, x.ItemCategoryId });
                    table.ForeignKey(
                        name: "FK_RulesetItemCategoryRule_Ruleset_RulesetId",
                        column: x => x.RulesetId,
                        principalTable: "Ruleset",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RulesetActionRule");

            migrationBuilder.DropTable(
                name: "RulesetItemCategoryRule");

            migrationBuilder.DropTable(
                name: "ScrimAction");

            migrationBuilder.DropTable(
                name: "Ruleset");
        }
    }
}
