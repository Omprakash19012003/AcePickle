using System.Collections;
using Microsoft.AspNetCore.SignalR;
using softskiller_chat_api.Helper;
using softskiller_chat_api.Models;
using softskiller_chat_api.Repository;
using static softskiller_chat_api.Dto.DTORoomResponse;

namespace softskiller_chat_api.Service
{
    public class ChatRoomService : IChatRoomService
    {
        private readonly IChatRoomRepository _chatroomrepository;
        private readonly IUserRepository _userrepository;
        private readonly IActiveDeviceRepository _activeDeviceRepository;
        private readonly NotificationService _notificationService;
        private readonly IHubContext<ChatHub.ChatHub> _hubContext;
        public ChatRoomService(IChatRoomRepository chatroomRepository, IUserRepository userrepository, NotificationService notificationService, IHubContext<ChatHub.ChatHub> hubContext,
                               IActiveDeviceRepository activeDeviceRepository)
        {
            _chatroomrepository = chatroomRepository;
            _userrepository = userrepository;
            _activeDeviceRepository = activeDeviceRepository;
            _notificationService = notificationService;
            _hubContext = hubContext;
        }
        public async Task<int> AddChatRoom(ChatRoom chatRoom)
        {
            return await _chatroomrepository.AddChatRoom(chatRoom);
        }
        public async Task<int> UpdateChatRoom(ChatRoom chatRoom)
        {
            return await _chatroomrepository.UpdateChatRoom(chatRoom);
        }

        public async Task<IEnumerable<object>> GetAllRoomsByUserId(int id, string searchKeyword)
        {
            return await _chatroomrepository.GetAllRoomsByUserId(id, searchKeyword);
        }

        public async Task<IEnumerable> GetAllGroupsByUserId(int userid, int? batchId, string searchKeyword)
        {
            return await _chatroomrepository.GetAllGroupsByUserId(userid, batchId, searchKeyword);
        }

        public async Task<ChatRoom> GetChatRoomById(int id)
        {
            return await _chatroomrepository.GetChatRoomById(id);
        }
        public async Task<ChatRoom> GetChatRoomByRoomId(string roomid)
        {
            return await _chatroomrepository.GetChatRoomByRoomId(roomid);
        }

        public async Task<bool> isChatRoomExist(string roomName)
        {
            return await _chatroomrepository.isChatRoomExist(roomName);
        }

        //ChatMessage
        public async Task<ChatMessages> AddMessages(ChatMessages chatMessage)
        {
            return await _chatroomrepository.AddMessages(chatMessage);
        }

        public async Task<MessageResponse> GetAllMessages(string roomId, int userId)
        {
            return await _chatroomrepository.GetAllMessages(roomId, userId);
        }

        //ChatRoom Mapping
        public async Task<int> ChatRoomMap(ChatRoomMapping chatRoomMapping)
        {
            return await _chatroomrepository.ChatRoomMap(chatRoomMapping);
        }

        public async Task<List<string>> GetAllRoomIdByUserId(int userid)
        {
            return await _chatroomrepository.GetAllRoomIdByUserId(userid);
        }

        public async Task<IEnumerable> GetAllUsersByRoomId(string groupId)
        {
            return await _chatroomrepository.GetAllUsersByRoomId(groupId);
        }

        public async Task<int> DeActivateUserFromGroup(string roomId, int userId)
        {
            if (roomId != null && userId != 0)
            {
                return await _chatroomrepository.DeActivateUserFromGroup(roomId, userId);
            }
            return 0;
        }

        public string RoomExist(int initiatedUserId, int joinedUserid)
        {
            return _chatroomrepository.RoomExist(initiatedUserId, joinedUserid);
        }

        public async Task<ChatRoomMapping> IsUserExistinRoom(string roomId, int userId)
        {
            return await _chatroomrepository.IsUserExistinRoom(roomId, userId);
        }

        public async Task<int> UpdateChatRoomMapping(ChatRoomMapping chatRoomMapping)
        {
            return await _chatroomrepository.UpdateChatRoomMapping(chatRoomMapping);
        }

        public async Task<int> RemoveUserFromGroup(string roomId, int userId, int contextUserId)
        {
            var userConnectionId = await _userrepository.GetConnectionIdByuserId(userId);
            var requestedUsername = await _userrepository.GetUserNamebyId(userId);

            if (userConnectionId != null)
            {
                await _hubContext.Groups.RemoveFromGroupAsync(userConnectionId, roomId);

                var messages = new List<Messages>
            {
                new Messages
                {
                    UserId = contextUserId,
                    Message = $"{requestedUsername} has been removed from the Group.",
                    CreatedDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
                }
            };

                var messageResponse = new MessageResponse
                {
                    RoomId = roomId,
                    RoomName = "",
                    Messages = messages
                };

                await _hubContext.Clients.Group(roomId).SendAsync("ReceiveMessage", messageResponse);
            }

            await DeActivateUserFromGroup(roomId, userId);

            var chatMessage = new ChatMessages
            {
                RoomId = roomId,
                UserId = contextUserId,
                Message = $"{requestedUsername} has been removed from the Group.",
                Status = 0,
                CreatedDate = DateTime.Now
            };

           var savedMessage = await AddMessages(chatMessage);
           return savedMessage.Id;
        }

        public async Task<int> AddNewUserToGroup(string roomId, string roomName, int userId, int contextUserId)
        {
            var userConnectionId = await _userrepository.GetConnectionIdByuserId(userId);
            var requestedUsername = await _userrepository.GetUserNamebyId(userId);
            var checkUserInRoom = await IsUserExistinRoom(roomId, userId);

            if (checkUserInRoom != null)
            {
                if (checkUserInRoom.Status == 1)
                {
                    // return new DTORoomResponse.DTORoomCreatedResponse { Message = "User already exists in Group.", RoomId = checkUserInRoom.RoomId };
                    return -1;
                }

                checkUserInRoom.Status = 1;
                await UpdateChatRoomMapping(checkUserInRoom);

                if (userConnectionId != null)
                {
                    await _hubContext.Groups.AddToGroupAsync(userConnectionId, roomId);

                    var messageResponse = new MessageResponse
                    {
                        RoomId = roomId,
                        RoomName = roomName,
                        Messages = new List<Messages>
                    {
                        new Messages
                        {
                            UserId = contextUserId,
                            Message = $"{requestedUsername} has joined the Group.",
                            CreatedDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
                        }
                    }
                    };

                    await _hubContext.Clients.Group(roomId).SendAsync("ReceiveMessage", messageResponse);
                    await _hubContext.Clients.Client(userConnectionId).SendAsync("NewGroup", new SendNewGroup
                    {
                        ChatRoomId = roomId,
                        ChatRoomName = roomName,
                        RoomType = 2,
                        CreatedDate = DateTime.Now
                    });
                }

                await AddMessages(new ChatMessages
                {
                    RoomId = roomId,
                    UserId = contextUserId,
                    Message = $"{requestedUsername} has joined the Group.",
                    Status = 0,
                    CreatedDate = DateTime.Now
                });

                return 1;
            }
            else
            {
                if (userConnectionId != null)
                {
                    await _hubContext.Groups.AddToGroupAsync(userConnectionId, roomId);
                    await _hubContext.Clients.Client(userConnectionId).SendAsync("NewGroup", new SendNewGroup
                    {
                        ChatRoomId = roomId,
                        ChatRoomName = roomName,
                        RoomType = 2,
                        CreatedDate = DateTime.Now
                    });
                }

                await AddMessages(new ChatMessages
                {
                    RoomId = roomId,
                    UserId = contextUserId,
                    Message = $"{requestedUsername} has joined the Group.",
                    Status = 0,
                    CreatedDate = DateTime.Now
                });

                await ChatRoomMap(new ChatRoomMapping
                {
                    RoomId = roomId,
                    UserId = userId,
                    RoomType = 2,
                    Status = 1,
                    CreatedDate = DateTime.Now
                });

                var messageResponse = new MessageResponse
                {
                    RoomId = roomId,
                    RoomName = roomName,
                    Messages = new List<Messages>
                {
                    new Messages
                    {
                        UserId = contextUserId,
                        Message = $"{requestedUsername} has joined the Group.",
                        CreatedDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
                    }
                }
                };

                await _hubContext.Clients.Group(roomId).SendAsync("ReceiveMessage", messageResponse);

                return 1;
            }
        }

        public async Task<int> GetUnreadCount(string roomId, int senderId)
        {
            return await _chatroomrepository.GetUnreadCount(roomId, senderId);
        }

        public async Task<int> ReadAllMessages(string roomId, int senderId)
        {
            var messages = await _chatroomrepository.GetUnreadMessages(roomId, senderId);

            messages.ForEach(m => m.Status = 1);
            
            return await _chatroomrepository.UpdateMessages(messages);
        }
        public async Task<int> SendPushNotification(string roomId, int senderId, string title, string body)
        {
            var userList = await _chatroomrepository.GetActiveUsersByRoomId(roomId, senderId);

            if (userList.Count == 0)
                return 0;

            var deviceTokens = await _activeDeviceRepository.GetActiveDeviceTokensByUserIds(userList);

            if (deviceTokens.Count == 0)
                return 0;

            var roomMapping = await _chatroomrepository.GetRoomMappingByRoomId(roomId);

            var notificationData = new NotificationData
            {
                RoomType  = roomMapping.RoomType,   // 1 = Private, 2 = Group
                RoomId    = roomId,
                SenderId  = senderId,
                BatchId   = roomMapping.BatchId,    
            };

            return await _notificationService.SendNotification(deviceTokens, title, body, notificationData);
        }
        public async Task<ChatMessages> GetMessageById(int messageId)
        {
            return await _chatroomrepository.GetMessageById(messageId);
        }
        public async Task<int> UpdateMessage(ChatMessages message)
        {
            return await _chatroomrepository.UpdateMessage(message);
        }
        public async Task<int> DeleteMessage(ChatMessages entity)
        {
            return await _chatroomrepository.DeleteMessage(entity);
        }



    }
}