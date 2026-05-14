using System.ComponentModel.DataAnnotations;

namespace softskiller_chat_api.Models
{
    public class ChatRoomMapping
    {
        [Key]
        [System.Text.Json.Serialization.JsonIgnore]
        public int Id { get; set; }
        public string RoomId { get; set; }
        public int UserId { get; set; }
        public int RoomType { get; set; }
        public sbyte Status { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}