using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.IO;

namespace squittal.ScrimPlanetmans.App.Migrations
{
    public partial class AddIsDeprecatedAndIsCurrentToMapRegionModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeprecated",
                table: "MapRegion",
                nullable: false,
                defaultValue: false);
            
            migrationBuilder.AddColumn<bool>(
                name: "IsCurrent",
                table: "MapRegion",
                nullable: false,
                defaultValue: false);

            var sqlFile = "Data/SQL/MigrationHelpers/Backfill_MapRegionsTable.sql";
            var basePath = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
            var filePath = Path.Combine(basePath, sqlFile);
            migrationBuilder.Sql(File.ReadAllText(filePath));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeprecated",
                table: "MapRegion");

            migrationBuilder.DropColumn(
                name: "IsCurrent",
                table: "MapRegion");
        }
    }
}
