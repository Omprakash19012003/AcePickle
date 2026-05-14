namespace acepickle_chat_api.Dto
{
    public class DTORoomResponse
    {
        public class DTORoomCreatedResponse
        {
            public string Message { get; set; }
            public string RoomId { get; set; }

        }
        public class SendNewuser
        {
            public int RequestedUserId { get; set; }
            public string? RequestedUsername { get; set; }
            public string? RequestedUserImageUrl { get; set; }
            public string? ChatRoomName { get; set; }
            public string ChatRoomId { get; set; }
            public int? RoomType { get; set; }
            public int? UserStatus { get; set; }
            public DateTime? CreatedDate { get; set; }
            public LastMessageDetails LastMessageDetails { get; set; }

        }

        public class LastMessageDetails
        {
            public string RoomId { get; set; }
            public int UserId { get; set; }
            public string Message { get; set; }
            public string CreatedDate { get; set; }
        }
        public class SendNewGroup
        {
            public string ChatRoomId { get; set; }
            public string ChatRoomName { get; set; }
            public string[] GroupUsers { get; set; }
            public int? RoomType { get; set; }
            public DateTime? CreatedDate { get; set; }
        }

        public class MessageResponse
        {
            public string? RoomId { get; set; }
            public string? RoomName { get; set; }
            public List<Messages> Messages { get; set; }

        }
        public class Messages
        {
            public int MessageId { get; set; }
            public int UserId { get; set; }
            public string? UserName { get; set; }
            public string? UserImage { get; set; }
            public string Message { get; set; }
            public int? UnreadMessageCount { get; set; }
            public string CreatedDate { get; set; }
            public bool IsEdited { get; set; }
        }
        public class MessageTyping
        {
            public int UserId { get; set; }
            public string RoomId { get; set; }
            public string Message { get; set; }
        }

        public class DTOCommonResponse
        {
            public string Message { get; set; }
            public Boolean Status { get; set; }

        }

        public class NotificationData
        {
            public int RoomType { get; set; }
            public string RoomId { get; set; }
            public int SenderId { get; set; }
            public int? BatchId { get; set; }
        }

        public class RoomMappingDto
        {
            public int RoomType { get; set; }
            public int? BatchId { get; set; }
        }
    }
}