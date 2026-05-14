namespace acepickle_chat_api.Helper
{
    public class AppSettings
    {
        public string JWTValidIssuer { get; set; }
        public string[] JWTValidAudiences { get; set; }
        public string JWTSecret { get; set; }
        public bool EnablePushNotificationForChat { get; set; }

    }

}
