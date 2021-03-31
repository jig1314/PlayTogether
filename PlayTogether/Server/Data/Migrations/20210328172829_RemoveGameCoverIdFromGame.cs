using Microsoft.EntityFrameworkCore.Migrations;

namespace PlayTogether.Server.Data.Migrations
{
    public partial class RemoveGameCoverIdFromGame : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ForeignKey_Game_GameCover",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Games_GameCoverId",
                table: "Games");

            migrationBuilder.DropPrimaryKey(
                name: "PrimaryKey_GameId",
                table: "GameCovers");

            migrationBuilder.DropColumn(
                name: "GameCoverId",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "GameCovers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GameCoverId",
                table: "Games",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GameId",
                table: "GameCovers",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PrimaryKey_GameId",
                table: "GameCovers",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_GameCoverId",
                table: "Games",
                column: "GameCoverId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_Game_GameCover",
                table: "Games",
                column: "GameCoverId",
                principalTable: "GameCovers",
                principalColumn: "GameId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
