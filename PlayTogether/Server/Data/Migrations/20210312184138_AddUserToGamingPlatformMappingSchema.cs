using Microsoft.EntityFrameworkCore.Migrations;

namespace PlayTogether.Server.Data.Migrations
{
    public partial class AddUserToGamingPlatformMappingSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationUser_GamingPlatform",
                columns: table => new
                {
                    ApplicationUserId = table.Column<string>(nullable: false),
                    GamingPlatformId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PrimaryKey_ApplicationUserId_GamingPlatformId", x => new { x.ApplicationUserId, x.GamingPlatformId });
                    table.ForeignKey(
                        name: "ForeignKey_User_GamingPlatform_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ForeignKey_User_GamingPlatform_GamingPlatformId",
                        column: x => x.GamingPlatformId,
                        principalTable: "GamingPlatforms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUser_GamingPlatform_GamingPlatformId",
                table: "ApplicationUser_GamingPlatform",
                column: "GamingPlatformId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUser_GamingPlatform");
        }
    }
}
