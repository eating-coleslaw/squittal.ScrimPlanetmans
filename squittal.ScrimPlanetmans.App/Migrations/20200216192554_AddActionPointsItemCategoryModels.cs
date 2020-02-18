using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace squittal.ScrimPlanetmans.App.Migrations
{
    public partial class AddActionPointsItemCategoryModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScrimRuleset",
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
                    table.PrimaryKey("PK_ScrimRuleset", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItemCategoryRule",
                columns: table => new
                {
                    RulesetId = table.Column<int>(nullable: false),
                    ItemCategoryId = table.Column<int>(nullable: false),
                    Points = table.Column<int>(nullable: false),
                    ScrimRulesetId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemCategoryRule", x => new { x.RulesetId, x.ItemCategoryId });
                    table.ForeignKey(
                        name: "FK_ItemCategoryRule_ScrimRuleset_ScrimRulesetId",
                        column: x => x.ScrimRulesetId,
                        principalTable: "ScrimRuleset",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScrimActionPoints",
                columns: table => new
                {
                    RulesetId = table.Column<int>(nullable: false),
                    Action = table.Column<int>(nullable: false),
                    Points = table.Column<int>(nullable: false),
                    ActionModelAction = table.Column<int>(nullable: true),
                    ScrimRulesetId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScrimActionPoints", x => new { x.RulesetId, x.Action });
                    table.ForeignKey(
                        name: "FK_ScrimActionPoints_ScrimAction_ActionModelAction",
                        column: x => x.ActionModelAction,
                        principalTable: "ScrimAction",
                        principalColumn: "Action",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScrimActionPoints_ScrimRuleset_ScrimRulesetId",
                        column: x => x.ScrimRulesetId,
                        principalTable: "ScrimRuleset",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemCategoryRule_ScrimRulesetId",
                table: "ItemCategoryRule",
                column: "ScrimRulesetId");

            migrationBuilder.CreateIndex(
                name: "IX_ScrimActionPoints_ActionModelAction",
                table: "ScrimActionPoints",
                column: "ActionModelAction");

            migrationBuilder.CreateIndex(
                name: "IX_ScrimActionPoints_ScrimRulesetId",
                table: "ScrimActionPoints",
                column: "ScrimRulesetId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemCategoryRule");

            migrationBuilder.DropTable(
                name: "ScrimActionPoints");

            migrationBuilder.DropTable(
                name: "ScrimRuleset");
        }
    }
}
