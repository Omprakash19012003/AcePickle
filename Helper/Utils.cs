using System.Net;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace skiller_api.Helper
{
    public class Utils
    {
        public static async Task<int> GetUserIDByCliam(HttpContext httpContext)
        {
            var identity = httpContext.User.Identity as ClaimsIdentity;
            return int.Parse(identity.FindFirst("id").Value);
        }
    }
}