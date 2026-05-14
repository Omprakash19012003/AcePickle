using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using softskiller_chat_api.Data;
using softskiller_chat_api.Models;

namespace softskiller_chat_api.Repository
{
    public class TimezoneRepository : ITimezoneRepository
    {
        private readonly MyDBContext _dbContext;

        public TimezoneRepository(MyDBContext dbContext)
        {
            _dbContext = dbContext;

        }

        public async Task<object> GetAllTimeZones()
        {
            var query = from timeZone in _dbContext.TimeZones
                        select new
                        {
                            id = timeZone.Id,
                            country = timeZone.Country,
                            timezone = timeZone.TimeZone
                        };

            return await query.ToListAsync();
        }
        public async Task<Timezone> GetTimezoneById(int id)
        {
            return await _dbContext.TimeZones.FindAsync(id);
        }

        private async Task<int> Save()
        {
            return await _dbContext.SaveChangesAsync();
        }


    }
}
