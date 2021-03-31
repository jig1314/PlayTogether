using Microsoft.EntityFrameworkCore.Migrations;

namespace PlayTogether.Server.Data.Migrations
{
    public partial class SeedApiIdData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                update GameGenres set ApiId = 4 where Name = 'Fighting'
                update GameGenres set ApiId = 35 where Name = 'Card & Board Game'
                update GameGenres set ApiId = 32 where Name = 'Indie'
                update GameGenres set ApiId = 34 where Name = 'Visual Novel'
                update GameGenres set ApiId = 33 where Name = 'Arcade'
                update GameGenres set ApiId = 31 where Name = 'Adventure'
                update GameGenres set ApiId = 30 where Name = 'Pinball'
                update GameGenres set ApiId = 25 where Name = 'Hack and slash/Beat ''em up'
                update GameGenres set ApiId = 26 where Name = 'Quiz/Trivia'
                update GameGenres set ApiId = 24 where Name = 'Tactical'
                update GameGenres set ApiId = 36 where Name = 'MOBA'
                update GameGenres set ApiId = 16 where Name = 'Turn-based strategy (TBS)'
                update GameGenres set ApiId = 14 where Name = 'Sport'
                update GameGenres set ApiId = 13 where Name = 'Simulator'
                update GameGenres set ApiId = 12 where Name = 'Role-playing (RPG)'
                update GameGenres set ApiId = 11 where Name = 'Real Time Strategy (RTS)'
                update GameGenres set ApiId = 10 where Name = 'Racing'
                update GameGenres set ApiId = 9 where Name = 'Puzzle'
                update GameGenres set ApiId = 8 where Name = 'Platform'
                update GameGenres set ApiId = 7 where Name = 'Music'
                update GameGenres set ApiId = 5 where Name = 'Shooter'
                update GameGenres set ApiId = 15 where Name = 'Strategy'
                update GameGenres set ApiId = 2 where Name = 'Point-and-click'

                update GamingPlatforms set ApiId = 37 where Slug = '3ds'
                update GamingPlatforms set ApiId = 82 where Slug = 'browser'
                update GamingPlatforms set ApiId = 33 where Slug = 'gb'
                update GamingPlatforms set ApiId = 24 where Slug = 'gba'
                update GamingPlatforms set ApiId = 22 where Slug = 'gbc'
                update GamingPlatforms set ApiId = 3 where Slug = 'linux'
                update GamingPlatforms set ApiId = 14 where Slug = 'mac'
                update GamingPlatforms set ApiId = 55 where Slug = 'mobile'
                update GamingPlatforms set ApiId = 4 where Slug = 'n64'
                update GamingPlatforms set ApiId = 20 where Slug = 'nds'
                update GamingPlatforms set ApiId = 18 where Slug = 'nes'
                update GamingPlatforms set ApiId = 21 where Slug = 'ngc'
                update GamingPlatforms set ApiId = 162 where Slug = 'oculus-vr'
                update GamingPlatforms set ApiId = 165 where Slug = 'playstation-vr'
                update GamingPlatforms set ApiId = 7 where Slug = 'ps'
                update GamingPlatforms set ApiId = 8 where Slug = 'ps2'
                update GamingPlatforms set ApiId = 9 where Slug = 'ps3'
                update GamingPlatforms set ApiId = 48 where Slug = 'ps4--1'
                update GamingPlatforms set ApiId = 167 where Slug = 'ps5'
                update GamingPlatforms set ApiId = 38 where Slug = 'psp'
                update GamingPlatforms set ApiId = 46 where Slug = 'psvita'
                update GamingPlatforms set ApiId = 169 where Slug = 'series-x'
                update GamingPlatforms set ApiId = 29 where Slug = 'smd'
                update GamingPlatforms set ApiId = 19 where Slug = 'snes--1'
                update GamingPlatforms set ApiId = 170 where Slug = 'stadia'
                update GamingPlatforms set ApiId = 163 where Slug = 'steam-vr'
                update GamingPlatforms set ApiId = 130 where Slug = 'switch'
                update GamingPlatforms set ApiId = 5 where Slug = 'wii'
                update GamingPlatforms set ApiId = 41 where Slug = 'wiiu'
                update GamingPlatforms set ApiId = 6 where Slug = 'win'
                update GamingPlatforms set ApiId = 161 where Slug = 'windows-mixed-reality'
                update GamingPlatforms set ApiId = 11 where Slug = 'xbox'
                update GamingPlatforms set ApiId = 12 where Slug = 'xbox360'
                update GamingPlatforms set ApiId = 49 where Slug = 'xboxone'
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                update GameGenres set ApiId = 0

                update GamingPlatforms set ApiId = 0
            ");
        }
    }
}
