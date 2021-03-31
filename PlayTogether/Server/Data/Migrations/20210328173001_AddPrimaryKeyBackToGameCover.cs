using Microsoft.EntityFrameworkCore.Migrations;

namespace PlayTogether.Server.Data.Migrations
{
    public partial class AddPrimaryKeyBackToGameCover : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GameId",
                table: "GameCovers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PrimaryKey_GameId",
                table: "GameCovers",
                column: "GameId");

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_GameCover_Game",
                table: "GameCovers",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ForeignKey_GameCover_Game",
                table: "GameCovers");

            migrationBuilder.DropPrimaryKey(
                name: "PrimaryKey_GameId",
                table: "GameCovers");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "GameCovers");
        }
    }
}
