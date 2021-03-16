using Microsoft.EntityFrameworkCore.Migrations;

namespace PlayTogether.Server.Data.Migrations
{
    public partial class SeedGameGenreData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql(@"
				insert into GameGenres (Name, Slug) 
				values 
                    ('Fighting', 'fighting'),
                    ('Card & Board Game', 'card-and-board-game'),
                    ('Indie', 'indie'),
                    ('Visual Novel', 'visual-novel'),
                    ('Arcade', 'arcade'),
                    ('Adventure', 'adventure'),
                    ('Pinball', 'pinball'),
                    ('Hack and slash/Beat ''em up', 'hack-and-slash-beat-em-up'),
                    ('Quiz/Trivia', 'quiz-trivia'),
                    ('Tactical', 'tactical'),
                    ('MOBA', 'moba'),
                    ('Turn-based strategy (TBS)', 'turn-based-strategy-tbs'),
                    ('Sport', 'sport'),
                    ('Simulator', 'simulator'),
                    ('Role-playing (RPG)', 'role-playing-rpg'),
                    ('Real Time Strategy (RTS)', 'real-time-strategy-rts'),
                    ('Racing', 'racing'),
                    ('Puzzle', 'puzzle'),
                    ('Platform', 'platform'),
                    ('Music', 'music'),
                    ('Shooter', 'shooter'),
                    ('Strategy', 'strategy'),
                    ('Point-and-click', 'point-and-click')
            ");
		}

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
