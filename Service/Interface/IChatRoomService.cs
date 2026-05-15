using System.Collections;
using acepickle_chat_api.Models;
using static acepickle_chat_api.Dto.DTORoomResponse;

namespace acepickle_chat_api.Service
{
    public interface IChatRoomService
    {
        Task<int> AddChatRoom(ChatRoom chatRoom);
        Task<int> UpdateChatRoom(ChatRoom chatRoom);
        Task<IEnumerable<object>> GetAllRoomsByUserId(int id, string searchKeyword);
        Task<IEnumerable> GetAllGroupsByUserId(int id, int? batchId, string searchKeyword);
        Task<ChatRoom> GetChatRoomById(int id);
        Task<ChatRoom> GetChatRoomByRoomId(string roomid);
        Task<bool> isChatRoomExist(string roomName);
        Task<int> RemoveUserFromGroup(string roomId, int userId, int contextUserId);
        Task<int> AddNewUserToGroup(string roomId, string roomName, int userId, int contextUserId);

        //ChatMessage 
        Task<MessageResponse> GetAllMessages(string roomId, int userId);
        Task<ChatMessages> AddMessages(ChatMessages chatMessage);
        Task<int> GetUnreadCount(string roomId, int senderId);
        Task<int> ReadAllMessages(string roomId, int senderId);

        //ChatRoom Mapping
        Task<int> ChatRoomMap(ChatRoomMapping chatRoomMapping);
        string RoomExist(int initiatedUserId, int joinedUserid);
        Task<List<string>> GetAllRoomIdByUserId(int userid);
        Task<IEnumerable> GetAllUsersByRoomId(string groupId);
        Task<int> DeActivateUserFromGroup(string roomId, int userId);
        Task<ChatRoomMapping> IsUserExistinRoom(string roomId, int userId);
        Task<int> UpdateChatRoomMapping(ChatRoomMapping chatRoomMapping);
        Task<int> SendPushNotification(string roomId, int senderId, string title, string body);
        Task<ChatMessages> GetMessageById(int messageId);
        Task<int> UpdateMessage(ChatMessages message);
        Task<int> DeleteMessage(ChatMessages entity);
    }
}