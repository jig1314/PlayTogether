using Microsoft.EntityFrameworkCore.Migrations;

namespace PlayTogether.Server.Data.Migrations
{
    public partial class FixGameSkillLevelForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ForeignKey_User_Game_GameSkillLevelId",
                table: "ApplicationUser_Games");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationUser_Games_GameSkillLevelId",
                table: "ApplicationUser_Games");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUser_Games_GameSkillLevelId",
                table: "ApplicationUser_Games",
                column: "GameSkillLevelId");

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_User_Game_GameSkillLevelId",
                table: "ApplicationUser_Games",
                column: "GameSkillLevelId",
                principalTable: "GameSkillLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ForeignKey_User_Game_GameSkillLevelId",
                table: "ApplicationUser_Games");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationUser_Games_GameSkillLevelId",
                table: "ApplicationUser_Games");

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
    }
}
