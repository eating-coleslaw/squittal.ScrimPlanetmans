using Microsoft.EntityFrameworkCore.Migrations;

namespace squittal.ScrimPlanetmans.App.Migrations
{
    public partial class AddRulesetFacilityRuleModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RulesetFacilityRule",
                columns: table => new
                {
                    RulesetId = table.Column<int>(nullable: false),
                    FacilityId = table.Column<int>(nullable: false),
                    MapRegionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RulesetFacilityRule", x => new { x.RulesetId, x.FacilityId });
                    table.ForeignKey(
                        name: "FK_RulesetFacilityRule_Ruleset_RulesetId",
                        column: x => x.RulesetId,
                        principalTable: "Ruleset",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RulesetFacilityRule_MapRegion_MapRegionId_FacilityId",
                        columns: x => new { x.MapRegionId, x.FacilityId },
                        principalTable: "MapRegion",
                        principalColumns: new[] { "Id", "FacilityId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RulesetFacilityRule_MapRegionId_FacilityId",
                table: "RulesetFacilityRule",
                columns: new[] { "MapRegionId", "FacilityId" },
                unique: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RulesetFacilityRule");
        }
    }
}
