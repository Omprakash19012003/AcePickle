
using softskiller_chat_api.Models;

namespace softskiller_chat_api.Services
{
    public interface ITimezoneService

    {
        Task<object> GetAllTimeZones();
        Task<Timezone> GetTimezoneById(int id);

    }
}
