using Microsoft.EntityFrameworkCore.Migrations;

namespace PlayTogether.Server.Data.Migrations
{
    public partial class SeedGamingPlatformData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
				insert into GamingPlatforms (Abbreviation, Name, Slug, LogoURL) 
				values 
					('PS2', 'PlayStation 2', 'ps2', 'https://images.igdb.com/igdb/image/upload/t_thumb/pl72.jpg'),
					('PC', 'PC (Microsoft Windows)', 'win', 'https://images.igdb.com/igdb/image/upload/t_thumb/irwvwpl023f8y19tidgq.jpg'),
					('PlayStation VR', 'PlayStation VR', 'playstation-vr', 'https://images.igdb.com/igdb/image/upload/t_thumb/niiex1gyepiu59aghpah.jpg'),
					('Steam VR', 'SteamVR', 'steam-vr', 'https://images.igdb.com/igdb/image/upload/t_thumb/ipbdzzx7z3rwuzm9big4.jpg'),
					('Switch', 'Nintendo Switch', 'switch', 'https://images.igdb.com/igdb/image/upload/t_thumb/pleu.jpg'),
					('XBOX', 'Xbox', 'xbox', 'https://images.igdb.com/igdb/image/upload/t_thumb/pl7e.jpg'),
					('GBA', 'Game Boy Advance', 'gba', 'https://images.igdb.com/igdb/image/upload/t_thumb/pl73.jpg'),
					('N64', 'Nintendo 64', 'n64', 'https://images.igdb.com/igdb/image/upload/t_thumb/pl78.jpg'),
					('NES', 'Nintendo Entertainment System (NES)', 'nes', 'https://images.igdb.com/igdb/image/upload/t_thumb/pl6d.jpg'),
					('GBC', 'Game Boy Color', 'gbc', 'https://images.igdb.com/igdb/image/upload/t_thumb/pl7l.jpg'),
					('WiiU', 'Wii U', 'wiiu', 'https://images.igdb.com/igdb/image/upload/t_thumb/pl6n.jpg'),
					('browser', 'Internet', 'browser', 'https://images.igdb.com/igdb/image/upload/t_thumb/plal.jpg'),
					('3DS', 'Nintendo 3DS', '3ds', 'https://images.igdb.com/igdb/image/upload/t_thumb/pl6o.jpg'),
					('Genesis', 'Sega Mega Drive/Genesis', 'smd', 'https://images.igdb.com/igdb/image/upload/t_thumb/pl85.jpg'),
					('PSP', 'PlayStation Portable', 'psp', 'https://images.igdb.com/igdb/image/upload/t_thumb/pl5y.jpg'),
					('SNES', 'SNES, Super Nintendo', 'snes--1', 'https://images.igdb.com/igdb/image/upload/t_thumb/plev.jpg'),
					('Mac', 'Mac', 'mac', 'https://images.igdb.com/igdb/image/upload/t_thumb/jl4t4o64uv2gizj2dxsy.jpg'),
					('NDS', 'Nintendo DS', 'nds', 'https://images.igdb.com/igdb/image/upload/t_thumb/pl6t.jpg'),
					('NGC', 'Nintendo GameCube', 'ngc', 'https://images.igdb.com/igdb/image/upload/t_thumb/pl7a.jpg'),
					('Oculus VR', 'Oculus VR', 'oculus-vr', 'https://images.igdb.com/igdb/image/upload/t_thumb/pivaofe9ll2b8cqfvvbu.jpg'),
					('PS3', 'PlayStation 3', 'ps3', 'https://images.igdb.com/igdb/image/upload/t_thumb/tuyy1nrqodtmbqajp4jg.jpg'),
					('Vita', 'PlayStation Vita', 'psvita', 'https://images.igdb.com/igdb/image/upload/t_thumb/pl6g.jpg'),
					('PS4', 'PlayStation 4', 'ps4--1', 'https://images.igdb.com/igdb/image/upload/t_thumb/pl6f.jpg'),
					('XONE', 'Xbox One', 'xboxone', 'https://images.igdb.com/igdb/image/upload/t_thumb/pl95.jpg'),
					('X360', 'Xbox 360', 'xbox360', 'https://images.igdb.com/igdb/image/upload/t_thumb/pl6y.jpg'),
					('PS5', 'PlayStation 5', 'ps5', 'https://images.igdb.com/igdb/image/upload/t_thumb/plcv.jpg'),
					('Mobile', 'Mobile', 'mobile', ''),
					('Wii', 'Wii', 'wii', 'https://images.igdb.com/igdb/image/upload/t_thumb/pl92.jpg'),
					('PS1', 'PlayStation', 'ps', 'https://images.igdb.com/igdb/image/upload/t_thumb/pl7q.jpg'),
					('Game Boy', 'Game Boy', 'gb', 'https://images.igdb.com/igdb/image/upload/t_thumb/pl7m.jpg'),
					('WMR', 'Windows Mixed Reality', 'windows-mixed-reality', ''),
					('Series X', 'Xbox Series X', 'series-x', 'https://images.igdb.com/igdb/image/upload/t_thumb/plfl.jpg'),
					('Linux', 'Linux', 'linux', 'https://images.igdb.com/igdb/image/upload/t_thumb/plak.jpg'),
					('Stadia', 'Google Stadia', 'stadia', 'https://images.igdb.com/igdb/image/upload/t_thumb/pl94.jpg')
            ");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
