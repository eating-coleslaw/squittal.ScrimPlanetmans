using Microsoft.EntityFrameworkCore.Migrations;

namespace squittal.ScrimPlanetmans.App.Migrations
{
    public partial class AddRulesetItemRuleModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DeferToItemRules",
                table: "RulesetItemCategoryRule",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBanned",
                table: "RulesetItemCategoryRule",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "RulesetItemRule",
                columns: table => new
                {
                    RulesetId = table.Column<int>(nullable: false),
                    ItemId = table.Column<int>(nullable: false),
                    ItemCategoryId = table.Column<int>(nullable: false),
                    Points = table.Column<int>(nullable: false, defaultValue: 0),
                    IsBanned = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RulesetItemRule", x => new { x.RulesetId, x.ItemId });
                    table.ForeignKey(
                        name: "FK_RulesetItemRule_Item_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Item",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RulesetItemRule_Ruleset_RulesetId",
                        column: x => x.RulesetId,
                        principalTable: "Ruleset",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RulesetItemRule_ItemId",
                table: "RulesetItemRule",
                column: "ItemId",
                unique: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RulesetItemRule");

            migrationBuilder.DropColumn(
                name: "DeferToItemRules",
                table: "RulesetItemCategoryRule");

            migrationBuilder.DropColumn(
                name: "IsBanned",
                table: "RulesetItemCategoryRule");
        }
    }
}
