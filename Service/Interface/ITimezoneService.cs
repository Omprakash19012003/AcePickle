
using acepickle_chat_api.Models;

namespace acepickle_chat_api.Services
{
    public interface ITimezoneService

    {
        Task<object> GetAllTimeZones();
        Task<Timezone> GetTimezoneById(int id);

    }
}
