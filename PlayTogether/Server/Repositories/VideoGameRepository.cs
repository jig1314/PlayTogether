using Microsoft.EntityFrameworkCore;
using PlayTogether.Server.Data;
using PlayTogether.Server.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlayTogether.Server.Repositories
{
    public class VideoGameRepository : IVideoGameRepository
    {
        private readonly ApplicationDbContext _context;

        public VideoGameRepository(ApplicationDbContext context)
        {
            this._context = context;
        }

        private async Task<(string clientId, string authorization)> GetHeaders()
        {
            var clientId = (await _context.AppSettings.FirstOrDefaultAsync(setting => setting.EnumCode == (int)AppSetting.IgdbClientId)).Value;
            var authorization = (await _context.AppSettings.FirstOrDefaultAsync(setting => setting.EnumCode == (int)AppSetting.IgdbAuthorization)).Value;

            return (clientId, authorization);
        }
    }
}
