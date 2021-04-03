using Microsoft.EntityFrameworkCore.Migrations;

namespace PlayTogether.Server.Data.Migrations
{
    public partial class SeedFriendRequestStatusTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
				insert into FriendRequestStatusTypes (EnumCode, Name, Description)
                values
	                (1, 'Sent', 'The friend request has been sent. The recipient has not accpeted/rejected the request yet.'),
	                (2, 'Accepted', 'The friend request has been accepted by the recipient.'),
	                (3, 'Rejected', 'The friend request has been rejected by the recipient.'),
	                (4, 'Cancelled', 'The friend request has been cancelled.')
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                delete from FriendRequestStatusTypes where EnumCode in (1,2,3,4)
            ");
        }
    }
}
