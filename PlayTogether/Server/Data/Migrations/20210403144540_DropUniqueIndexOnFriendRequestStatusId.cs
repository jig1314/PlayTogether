using Microsoft.EntityFrameworkCore.Migrations;

namespace PlayTogether.Server.Data.Migrations
{
    public partial class DropUniqueIndexOnFriendRequestStatusId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FriendRequests_FriendRequestStatusId",
                table: "FriendRequests");

            migrationBuilder.CreateIndex(
                name: "IX_FriendRequests_FriendRequestStatusId",
                table: "FriendRequests",
                column: "FriendRequestStatusId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FriendRequests_FriendRequestStatusId",
                table: "FriendRequests");

            migrationBuilder.CreateIndex(
                name: "IX_FriendRequests_FriendRequestStatusId",
                table: "FriendRequests",
                column: "FriendRequestStatusId",
                unique: true);
        }
    }
}
