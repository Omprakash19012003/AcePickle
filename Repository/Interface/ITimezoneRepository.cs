
using softskiller_chat_api.Models;

namespace softskiller_chat_api.Repository
{
    public interface  ITimezoneRepository

    {
         Task<object> GetAllTimeZones();
         Task<Timezone> GetTimezoneById(int id);
        
    }
}
