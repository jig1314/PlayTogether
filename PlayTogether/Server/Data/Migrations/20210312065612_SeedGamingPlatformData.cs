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
					('Switch', 'Nintendo Switch', 'switch', 'https://logodix.com/logo/515874.jpg'),
					('XBOX', 'Xbox', 'xbox', 'https://images.igdb.com/igdb/image/upload/t_thumb/pl7e.jpg'),
					('GBA', 'Game Boy Advance', 'gba', 'https://images.igdb.com/igdb/image/upload/t_thumb/pl73.jpg'),
					('N64', 'Nintendo 64', 'n64', 'https://1000logos.net/wp-content/uploads/2017/07/Emblem-N64.jpg'),
					('NES', 'Nintendo Entertainment System (NES)', 'nes', 'https://images.igdb.com/igdb/image/upload/t_thumb/pl6d.jpg'),
					('GBC', 'Game Boy Color', 'gbc', 'https://images.igdb.com/igdb/image/upload/t_thumb/pl7l.jpg'),
					('WiiU', 'Wii U', 'wiiu', 'https://images.igdb.com/igdb/image/upload/t_thumb/pl6n.jpg'),
					('browser', 'Internet', 'browser', 'https://images.igdb.com/igdb/image/upload/t_thumb/plal.jpg'),
					('3DS', 'Nintendo 3DS', '3ds', 'https://images.igdb.com/igdb/image/upload/t_thumb/pl6o.jpg'),
					('Genesis', 'Sega Mega Drive/Genesis', 'smd', 'https://images.igdb.com/igdb/image/upload/t_thumb/pl85.jpg'),
					('PSP', 'PlayStation Portable', 'psp', 'https://cdn.shopify.com/s/files/1/1447/1736/files/playstation-portable-psp-logo_480x480.png'),
					('SNES', 'SNES, Super Nintendo', 'snes--1', 'https://logodix.com/logo/310991.jpg'),
					('Mac', 'Mac', 'mac', 'https://brandeps.com/logo-download/A/Apple-logo-vector-01.svg'),
					('NDS', 'Nintendo DS', 'nds', 'https://images.igdb.com/igdb/image/upload/t_thumb/pl6t.jpg'),
					('NGC', 'Nintendo GameCube', 'ngc', 'https://images.igdb.com/igdb/image/upload/t_thumb/pl7a.jpg'),
					('Oculus VR', 'Oculus VR', 'oculus-vr', 'http://www.logo-designer.co/wp-content/uploads/2015/06/Oculus-Rift-new-logo-design-10.png'),
					('PS3', 'PlayStation 3', 'ps3', 'https://logoeps.com/wp-content/uploads/2011/06/sony-ps3-slim-logo-vector-01.png'),
					('Vita', 'PlayStation Vita', 'psvita', 'https://images.igdb.com/igdb/image/upload/t_thumb/pl6g.jpg'),
					('PS4', 'PlayStation 4', 'ps4--1', 'https://images.igdb.com/igdb/image/upload/t_thumb/pl6f.jpg'),
					('XONE', 'Xbox One', 'xboxone', 'https://images.igdb.com/igdb/image/upload/t_thumb/pl95.jpg'),
					('X360', 'Xbox 360', 'xbox360', 'https://images.igdb.com/igdb/image/upload/t_thumb/pl6y.jpg'),
					('PS5', 'PlayStation 5', 'ps5', 'https://cdn.mos.cms.futurecdn.net/cLf6g4KQr4FyX9ufAmxuW6-970-80.jpg'),
					('Mobile', 'Mobile', 'mobile', 'https://logodix.com/logo/366982.jpg'),
					('Wii', 'Wii', 'wii', 'https://img.hexus.net/v2/internationalevents/e3/wii_logo.jpg'),
					('PS1', 'PlayStation', 'ps', 'https://images.igdb.com/igdb/image/upload/t_thumb/pl7q.jpg'),
					('Game Boy', 'Game Boy', 'gb', 'https://images.igdb.com/igdb/image/upload/t_thumb/pl7m.jpg'),
					('WMR', 'Windows Mixed Reality', 'windows-mixed-reality', 'https://store-images.s-microsoft.com/image/apps.4422.13588442905826814.795a5ce2-2721-4e5d-af2f-c7be66360dcd.0e33b464-cd02-4440-a53e-bbe9b7f03d72?mode=scale&q=90&h=300&w=300'),
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
