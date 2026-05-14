using System.ComponentModel.DataAnnotations;

namespace acepickle_chat_api.Models
{
    public class ActiveUser
    {
        [Key]
        [System.Text.Json.Serialization.JsonIgnore]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ConnectionId { get; set; }
        public sbyte UserStatus { get; set; }
        public DateTime LastOnline { get; set; }
    }
}