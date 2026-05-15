
using acepickle_chat_api.Models;

namespace acepickle_chat_api.Repository
{
    public interface ITimezoneRepository

    {
        Task<object> GetAllTimeZones();
        Task<Timezone> GetTimezoneById(int id);

    }
}
