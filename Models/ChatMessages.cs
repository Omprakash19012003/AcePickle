using System.ComponentModel.DataAnnotations;

namespace acepickle_chat_api.Models
{
    public class ChatMessages
    {
        [Key]
        [System.Text.Json.Serialization.JsonIgnore]
        public int Id { get; set; }
        public string RoomId { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; }
        public sbyte? Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsEdited { get; set; } = false;
        public DateTime? UpdatedDate { get; set; }
    }
}