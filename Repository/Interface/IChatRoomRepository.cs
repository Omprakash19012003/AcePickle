using System.Collections;
using acepickle_chat_api.Models;
using static acepickle_chat_api.Dto.DTORoomResponse;

namespace acepickle_chat_api.Repository
{
    public interface IChatRoomRepository
    {
        Task<int> AddChatRoom(ChatRoom chatRoom);
        Task<int> UpdateChatRoom(ChatRoom chatRoom);
        Task<IEnumerable<object>> GetAllRoomsByUserId(int id, string searchKeyword);
        Task<IEnumerable> GetAllGroupsByUserId(int userid, int? batchId, string? searchkeyword);
        Task<ChatRoom> GetChatRoomById(int id);
        Task<ChatRoom> GetChatRoomByRoomId(string roomid);
        Task<bool> isChatRoomExist(string roomName);

        //ChatMessage
        Task<MessageResponse> GetAllMessages(string roomId, int userId);
        Task<ChatMessages> AddMessages(ChatMessages chatMessage);
        Task<int> GetUnreadCount(string roomId, int senderId);
        Task<int> UpdateMessages(List<ChatMessages> chatMessage);
        Task<List<ChatMessages>> GetUnreadMessages(string roomId, int senderId);

        //ChatRoomMapping
        Task<int> ChatRoomMap(ChatRoomMapping chatRoomMapping);
        string RoomExist(int initiatedUserId, int joinedUserid);
        Task<List<string>> GetAllRoomIdByUserId(int userid);
        Task<IEnumerable> GetAllUsersByRoomId(string groupId);
        Task<int> DeActivateUserFromGroup(string roomId, int userId);
        Task<ChatRoomMapping> IsUserExistinRoom(string roomId, int userId);
        Task<int> UpdateChatRoomMapping(ChatRoomMapping chatRoomMapping);
        Task<List<int>> GetActiveUsersByRoomId(string roomId, int currentUserId);
        Task<ChatMessages> GetMessageById(int messageId);
        Task<int> UpdateMessage(ChatMessages message);
        Task<int> DeleteMessage(ChatMessages entity);
        Task<RoomMappingDto> GetRoomMappingByRoomId(string roomId);
    }
}