using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.IO;

namespace squittal.ScrimPlanetmans.App.Migrations
{
    public partial class AddScrimMatchInfoView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sqlFile = "Data/SQL/Views/View_ScrimMatchInfo.sql";
            var basePath = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
            var filePath = Path.Combine(basePath, sqlFile);
            migrationBuilder.Sql(File.ReadAllText(filePath));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
