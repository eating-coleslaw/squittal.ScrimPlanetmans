using Microsoft.EntityFrameworkCore.Migrations;

namespace squittal.ScrimPlanetmans.App.Migrations
{
    public partial class AddScrimActionTypeDomainToModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Domain",
                table: "ScrimAction",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ScrimActionTypeDomain",
                table: "RulesetActionRule",
                nullable: false,
                defaultValue: -1);

            migrationBuilder.CreateIndex(
                name: "IX_RulesetItemCategoryRule_ItemCategoryId",
                table: "RulesetItemCategoryRule",
                column: "ItemCategoryId",
                unique: false);

            migrationBuilder.AddForeignKey(
                name: "FK_RulesetItemCategoryRule_ItemCategory_ItemCategoryId",
                table: "RulesetItemCategoryRule",
                column: "ItemCategoryId",
                principalTable: "ItemCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RulesetItemCategoryRule_ItemCategory_ItemCategoryId",
                table: "RulesetItemCategoryRule");

            migrationBuilder.DropIndex(
                name: "IX_RulesetItemCategoryRule_ItemCategoryId",
                table: "RulesetItemCategoryRule");

            migrationBuilder.DropColumn(
                name: "Domain",
                table: "ScrimAction");

            migrationBuilder.DropColumn(
                name: "ScrimActionTypeDomain",
                table: "RulesetActionRule");
        }
    }
}
