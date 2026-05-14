using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace softskiller_chat_api.Models
{
    public class ActiveDevices
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PlatformId { get; set; }
        public string DeviceToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Expirytime { get; set; }
        public string? IpAddress { get; set; }
        public string? Location { get; set; }
        public sbyte? ActiveStatus { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime UpdatedTime { get; set; }
    }
}