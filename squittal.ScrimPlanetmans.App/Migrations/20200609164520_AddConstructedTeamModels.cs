using Microsoft.EntityFrameworkCore.Migrations;

namespace squittal.ScrimPlanetmans.App.Migrations
{
    public partial class AddConstructedTeamModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConstructedTeam",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    Alias = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConstructedTeam", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConstructedTeamPlayerMembership",
                columns: table => new
                {
                    ConstructedTeamId = table.Column<int>(nullable: false),
                    CharacterId = table.Column<string>(nullable: false),
                    FactionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConstructedTeamPlayerMembership", x => new { x.ConstructedTeamId, x.CharacterId });
                    table.ForeignKey(
                        name: "FK_ConstructedTeamPlayerMembership_ConstructedTeam_ConstructedTeamId",
                        column: x => x.ConstructedTeamId,
                        principalTable: "ConstructedTeam",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConstructedTeamPlayerMembership");

            migrationBuilder.DropTable(
                name: "ConstructedTeam");
        }
    }
}
