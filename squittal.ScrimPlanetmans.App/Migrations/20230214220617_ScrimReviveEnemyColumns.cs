using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.IO;

namespace squittal.ScrimPlanetmans.App.Migrations
{
    public partial class ScrimReviveEnemyColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EnemyActionType",
                table: "ScrimRevive",
                nullable: false,
                defaultValue: 9001);

            migrationBuilder.AddColumn<int>(
                name: "EnemyPoints",
                table: "ScrimRevive",
                nullable: false,
                defaultValue: 0);

            var sqlFile = "Data/SQL/MigrationHelpers/Backfill_ScrimReviveEnemyActionType.sql";
            var basePath = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
            var filePath = Path.Combine(basePath, sqlFile);
            migrationBuilder.Sql(File.ReadAllText(filePath));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnemyActionType",
                table: "ScrimRevive");

            migrationBuilder.DropColumn(
                name: "EnemyPoints",
                table: "ScrimRevive");
        }
    }
}
