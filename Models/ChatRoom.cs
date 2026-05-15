using System.ComponentModel.DataAnnotations;

namespace acepickle_chat_api.Models
{
    public class ChatRoom
    {
        [Key]
        [System.Text.Json.Serialization.JsonIgnore]
        public int Id { get; set; }
        public string RoomId { get; set; }
        public int? BatchId { get; set; }
        public string? RoomName { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}