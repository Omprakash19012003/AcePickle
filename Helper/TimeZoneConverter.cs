
using acepickle_chat_api.Services;

namespace acepickle_chat_api.Helper
{
    public class TimeZoneConverter
    {
        private readonly ITimezoneService _timezoneService;
        public TimeZoneConverter(ITimezoneService timezoneService)
        {
            _timezoneService = timezoneService;
        }
        public async Task<DateTime> GetUtcDateTimeByTimeZoneId(DateTime dateTime, int timeZoneId)
        {
            var timeZone = await _timezoneService.GetTimezoneById(timeZoneId);

            char sign = timeZone.Offset[3];

            int hoursOffset = int.Parse(timeZone.Offset.Substring(4, 2));
            int minutesOffset = int.Parse(timeZone.Offset.Substring(7, 2));

            int totalOffset = hoursOffset * 60 + minutesOffset;

            if (sign == '+')
            {
                dateTime = dateTime.AddMinutes(-totalOffset);
            }
            else if (sign == '-')
            {
                dateTime = dateTime.AddMinutes(totalOffset);
            }

            return dateTime;
        }

        // public async Task<DateTime> GetCurrentTimeByZoneId(DateTime dateTime, int timeZoneId)
        // {
        //     var timeZone = await _timezoneService.GetTimezoneById(timeZoneId);

        //     char sign = timeZone.Offset[3];

        //     int hoursOffset = int.Parse(timeZone.Offset.Substring(4, 2));
        //     int minutesOffset = int.Parse(timeZone.Offset.Substring(7, 2));

        //     int totalOffset = hoursOffset * 60 + minutesOffset;

        //     if (sign == '+')
        //     {
        //         dateTime = dateTime.AddMinutes(totalOffset);
        //     }
        //     else if (sign == '-')
        //     {
        //         dateTime = dateTime.AddMinutes(-totalOffset);
        //     }

        //     return dateTime;
        // }

        public async Task<DateTime> GetCurrentTimeByZoneId(DateTime dateTime, int timeZoneId)
        {
            if (dateTime.Kind == DateTimeKind.Unspecified)
                dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            else if (dateTime.Kind == DateTimeKind.Local)
                dateTime = dateTime.ToUniversalTime();

            var timeZone = await _timezoneService.GetTimezoneById(timeZoneId);

            char sign = timeZone.Offset[3];
            int hoursOffset = int.Parse(timeZone.Offset.Substring(4, 2));
            int minutesOffset = int.Parse(timeZone.Offset.Substring(7, 2));

            int totalMinutes = hoursOffset * 60 + minutesOffset;

            if (sign == '+')
                dateTime = dateTime.AddMinutes(totalMinutes);
            else
                dateTime = dateTime.AddMinutes(-totalMinutes);

            return DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
        }

    }
}