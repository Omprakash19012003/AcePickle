using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using softskiller_chat_api.Data;
using softskiller_chat_api.Models;

namespace softskiller_chat_api.Repository
{
    public class ActiveDeviceRepository : IActiveDeviceRepository
    {
        private readonly MyDBContext _dbContext;

        public ActiveDeviceRepository(MyDBContext dbContext)
        {
            _dbContext = dbContext;

        }

        public async Task<int> AddActiveDevice(ActiveDevices activeDevices)
        {
            _dbContext.ActiveDevices.Add(activeDevices);
            return await Save();
        }

        public async Task<int> UpdateActiveDevice(ActiveDevices activeDevices)
        {
            _dbContext.ActiveDevices.Update(activeDevices);
            return await Save();
        }

        public async Task<ActiveDevices> GetActiveDeviceByUserId(int userid, int platformid)
        {
            return await _dbContext.ActiveDevices.FirstOrDefaultAsync(x => x.UserId == userid && x.PlatformId == platformid);
        }
        public async Task<List<string>> GetActiveDeviceTokensByUserId(int userid)
        {
            return await _dbContext.ActiveDevices.Where(x => x.UserId == userid).Select(x => x.DeviceToken).ToListAsync();
        }

        public async Task<List<string>> GetActiveDeviceTokensByUserIds(List<int> userIds)
        {
            return await _dbContext.ActiveDevices
                                   .Where(x => userIds.Contains(x.UserId))
                                   .Select(x => x.DeviceToken)
                                   .ToListAsync();
        }

        public async Task<int> DeleteActiveDeviceById(int id)
        {

            ActiveDevices result = await _dbContext.ActiveDevices.FindAsync(id);
            if (result != null)
            {
                _dbContext.ActiveDevices.Remove(result);
                return await Save();
            }
            return 0;
        }

        private async Task<int> Save()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}