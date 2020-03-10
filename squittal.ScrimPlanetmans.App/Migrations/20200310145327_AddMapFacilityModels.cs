using Microsoft.EntityFrameworkCore.Migrations;

namespace squittal.ScrimPlanetmans.App.Migrations
{
    public partial class AddMapFacilityModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FacilityType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilityType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MapRegion",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    FacilityId = table.Column<int>(nullable: false),
                    FacilityName = table.Column<string>(nullable: true),
                    FacilityTypeId = table.Column<int>(nullable: false),
                    FacilityType = table.Column<string>(nullable: true),
                    ZoneId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapRegion", x => new { x.Id, x.FacilityId });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FacilityType");

            migrationBuilder.DropTable(
                name: "MapRegion");
        }
    }
}
