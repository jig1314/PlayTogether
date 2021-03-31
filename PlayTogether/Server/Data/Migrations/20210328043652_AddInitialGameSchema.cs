using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PlayTogether.Server.Data.Migrations
{
    public partial class AddInitialGameSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameCovers",
                columns: table => new
                {
                    GameId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Height = table.Column<int>(type: "int", nullable: false),
                    Width = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PrimaryKey_GameId", x => x.GameId);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReleaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GameCoverId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "ForeignKey_Game_GameCover",
                        column: x => x.GameCoverId,
                        principalTable: "GameCovers",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationUser_Games",
                columns: table => new
                {
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GameId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PrimaryKey_ApplicationUserId_GameId", x => new { x.ApplicationUserId, x.GameId });
                    table.ForeignKey(
                        name: "ForeignKey_User_Game_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ForeignKey_User_Game_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameGenre_Games",
                columns: table => new
                {
                    GameGenreId = table.Column<int>(type: "int", nullable: false),
                    GameId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PrimaryKey_GameGenreId_GameId", x => new { x.GameGenreId, x.GameId });
                    table.ForeignKey(
                        name: "ForeignKey_GameGenre_Game_GameGenreId",
                        column: x => x.GameGenreId,
                        principalTable: "GameGenres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ForeignKey_GameGenre_Game_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GamingPlatform_Games",
                columns: table => new
                {
                    GamingPlatformId = table.Column<int>(type: "int", nullable: false),
                    GameId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PrimaryKey_GamingPlatformId_GameId", x => new { x.GamingPlatformId, x.GameId });
                    table.ForeignKey(
                        name: "ForeignKey_GamingPlatform_Game_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ForeignKey_GamingPlatform_Game_GamingPlatformId",
                        column: x => x.GamingPlatformId,
                        principalTable: "GamingPlatforms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUser_Games_GameId",
                table: "ApplicationUser_Games",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GameGenre_Games_GameId",
                table: "GameGenre_Games",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_GameCoverId",
                table: "Games",
                column: "GameCoverId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GamingPlatform_Games_GameId",
                table: "GamingPlatform_Games",
                column: "GameId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUser_Games");

            migrationBuilder.DropTable(
                name: "GameGenre_Games");

            migrationBuilder.DropTable(
                name: "GamingPlatform_Games");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "GameCovers");
        }
    }
}
