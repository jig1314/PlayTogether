using Microsoft.EntityFrameworkCore.Migrations;

namespace PlayTogether.Server.Data.Migrations
{
    public partial class SeedGameSkillLevelData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
				insert into GameSkillLevels (Name, Description) 
				values 
                    ('Beginner', 'Haven’t played much, still learning'),
                    ('Experienced', 'Good at the game, but have room for improvement'),
                    ('Veteran', 'Advanced player who can compete against anyone'),
                    ('Pro', 'Best of the best, an elite player')
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
				delete from GameSkillLevels where Name in ('Beginner', 'Experienced', 'Veteran', 'Pro')
            ");
        }
    }
}
