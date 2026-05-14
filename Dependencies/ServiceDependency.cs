
using acepickle_chat_api.Helper;
using acepickle_chat_api.Repository;
using acepickle_chat_api.Service;
using acepickle_chat_api.Services;

namespace acepickle_chat_api.Dependencies
{
    public static class ServiceDependency
    {
        public static void AddServiceDependency(this IServiceCollection service)
        {
            service.AddTransient<IJwtTokenService, JwtTokenService>();
            service.AddTransient<IChatRoomRepository, ChatRoomRepository>();
            service.AddTransient<IChatRoomService, ChatRoomService>();
            service.AddTransient<IUserRepository, UserRepository>();
            service.AddTransient<IUserService, UserService>();
            service.AddTransient<IActiveDeviceRepository, ActiveDeviceRepository>();


            service.AddTransient<ITimezoneRepository, TimezoneRepository>();
            service.AddTransient<ITimezoneService, TimezoneService>();
            service.AddTransient<TimeZoneConverter>();
            service.AddTransient<NotificationService>();

        }
    }
}