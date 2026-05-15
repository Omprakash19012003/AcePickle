using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using acepickle_chat_api.Models;

namespace acepickle_chat_api.Repository
{
    public interface IActiveDeviceRepository
    {
        Task<int> AddActiveDevice(ActiveDevices activeDevices);
        Task<int> UpdateActiveDevice(ActiveDevices activeDevices);
        Task<ActiveDevices> GetActiveDeviceByUserId(int userid, int platformid);
        Task<List<string>> GetActiveDeviceTokensByUserId(int userid);
        Task<int> DeleteActiveDeviceById(int id);
        Task<List<string>> GetActiveDeviceTokensByUserIds(List<int> userIds);
    }
}