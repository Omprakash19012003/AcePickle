

using softskiller_chat_api.Models;
using softskiller_chat_api.Repository;
using softskiller_chat_api.Services;

namespace softskiller_chat_api.Service
{
    public class TimezoneService : ITimezoneService
    {
        private readonly ITimezoneRepository _timezoneRepository;


        public TimezoneService(ITimezoneRepository timezoneRepository)
        {
            _timezoneRepository = timezoneRepository;
        }

        public async Task<object> GetAllTimeZones()
        {
            return await _timezoneRepository.GetAllTimeZones();
        }

        public async Task<Timezone> GetTimezoneById(int id)
        {
            return await _timezoneRepository.GetTimezoneById(id);
        }
    }
}
