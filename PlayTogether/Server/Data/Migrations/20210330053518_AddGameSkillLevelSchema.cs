using Microsoft.EntityFrameworkCore.Migrations;

namespace PlayTogether.Server.Data.Migrations
{
    public partial class AddGameSkillLevelSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GameSkillLevelId",
                table: "ApplicationUser_Games",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GameSkillLevels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSkillLevels", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUser_Games_GameSkillLevelId",
                table: "ApplicationUser_Games",
                column: "GameSkillLevelId",
                unique: true,
                filter: "[GameSkillLevelId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_User_Game_GameSkillLevelId",
                table: "ApplicationUser_Games",
                column: "GameSkillLevelId",
                principalTable: "GameSkillLevels",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ForeignKey_User_Game_GameSkillLevelId",
                table: "ApplicationUser_Games");

            migrationBuilder.DropTable(
                name: "GameSkillLevels");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationUser_Games_GameSkillLevelId",
                table: "ApplicationUser_Games");

            migrationBuilder.DropColumn(
                name: "GameSkillLevelId",
                table: "ApplicationUser_Games");
        }
    }
}
