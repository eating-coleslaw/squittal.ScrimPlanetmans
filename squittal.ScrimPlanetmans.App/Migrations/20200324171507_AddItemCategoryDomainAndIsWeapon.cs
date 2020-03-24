using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.IO;

namespace squittal.ScrimPlanetmans.App.Migrations
{
    public partial class AddItemCategoryDomainAndIsWeapon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Domain",
                table: "ItemCategory",
                nullable: false,
                defaultValue: -1);

            migrationBuilder.AddColumn<bool>(
                name: "IsWeaponCategory",
                table: "ItemCategory",
                nullable: false,
                defaultValue: false);

            var sqlFile = "Data/SQL/MigrationHelpers/Backfill_ItemCategoryDomainAndIsWeapon.sql";
            var basePath = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
            var filePath = Path.Combine(basePath, sqlFile);
            migrationBuilder.Sql(File.ReadAllText(filePath));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Domain",
                table: "ItemCategory");

            migrationBuilder.DropColumn(
                name: "IsWeaponCategory",
                table: "ItemCategory");
        }
    }
}
