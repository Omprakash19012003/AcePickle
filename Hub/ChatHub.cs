using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using acepickle_chat_api.Dto;
using acepickle_chat_api.Helper;
using acepickle_chat_api.Models;
using acepickle_chat_api.Service;
using static acepickle_chat_api.Dto.DTORoomResponse;

namespace acepickle_chat_api.ChatHub
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChatHub : Hub
    {
        private static List<SendNewuser> ConnectedUsers = new List<SendNewuser>();
        private readonly IChatRoomService _chatroomService;
        private readonly IUserService _userService;
        private readonly TimeZoneConverter _timezoneConverterService;
        private readonly AppSettings _appSettings;

        // ***
        // Step 1
        // When you call this endpoint http://appdomain.com/chatHub it will be authenticated via JWT token and websocket connection will be established.
        // On connected from the client side we need to call two api's to get the User Room List & Chat messages.  
        // GetAllRoomsByUserId() -- User specific rooms will be pushed to client side.
        // GetAllMessagesByRoomId(string roomId) -- Room specific messages will be pushed to client side.
        // GetAllGroupsByUserId() -- User specific Groups will be pushed to client side , When switched to Groupchat.
        // GetAllUsersByGroupId(string groupId) -- Group (or) Room specified users will be pushed to client side.
        //
        // -- HUB METHODS --
        // 1. CreateRoom(int initiatedUserid, int requestedUserId) 
        // 2. CreateGroup(string roomName, List<int> userids)
        // 3. AddNewUsertoGroup(string roomId, string roomName, int userid)
        // 4. RemoveUserFromGroup(string roomId, int userid)
        // 5. SendMessage(string roomId, int senderId, string message)
        // 6. OnConnectedAsync()
        // 7. OnDisconnectedAsync(Exception exception)
        // ***
        // You can only use the Hub method by "INVOKING" in the client side.
        // Want to know more about SignalR Hub --> https://learn.microsoft.com/en-us/aspnet/core/signalr/hubs?view=aspnetcore-7.0
        public ChatHub(IChatRoomService chatroomService, IUserService userService, TimeZoneConverter timezoneConverterService, IOptions<AppSettings> appSettings)
        {
            _chatroomService = chatroomService;
            _userService = userService;
            _timezoneConverterService = timezoneConverterService;
            _appSettings = appSettings.Value;
        }

        // CreateRoom -- This method is used to create a chat between two users by their userId's , When invoked in client side(Private Chat).
        public async Task<DTORoomResponse.DTORoomCreatedResponse> CreateRoom(int initiatedUserid, int requestedUserId)
        {
            var roomExist = _chatroomService.RoomExist(initiatedUserid, requestedUserId);

            if (roomExist != null)
            {
                return new DTORoomResponse.DTORoomCreatedResponse { Message = "Room already Exist..", RoomId = roomExist.ToString() };
            }
            else
            {
                Guid roomID = Guid.NewGuid();

                await Groups.AddToGroupAsync(Context.ConnectionId, roomID.ToString());
                var initiatedUserInfo = await _userService.IsUserExist(initiatedUserid);

                var nowForInitiatedUser = await _timezoneConverterService
                    .GetCurrentTimeByZoneId(DateTime.UtcNow, (int)initiatedUserInfo.TimeZoneId);

                var roomDetail = new ChatRoom
                {
                    RoomId = roomID.ToString(),
                    BatchId = 0,
                    RoomName = requestedUserId.ToString(),
                    CreatedDate = nowForInitiatedUser
                };

                await _chatroomService.AddChatRoom(roomDetail);

                var roomMapping = new ChatRoomMapping
                {
                    RoomId = roomID.ToString(),
                    UserId = initiatedUserid,
                    RoomType = 1,
                    Status = 1,
                    CreatedDate = nowForInitiatedUser
                };

                await _chatroomService.ChatRoomMap(roomMapping);

                var roomMappinganother = new ChatRoomMapping
                {
                    RoomId = roomID.ToString(),
                    UserId = requestedUserId,
                    RoomType = 1,
                    Status = 1,
                    CreatedDate = nowForInitiatedUser
                };

                await _chatroomService.ChatRoomMap(roomMappinganother);

                var roomExistinlocal = ConnectedUsers.Find(x => x.ChatRoomId == roomExist);

                if (roomExistinlocal != null)
                {
                    ConnectedUsers.Remove(roomExistinlocal);
                }
                var requestedUsername = await _userService.GetUserNamebyId(requestedUserId);

                var requestedUserImageUrl = await _userService.GetUserPicturebyId(requestedUserId);

                var userStatus = await _userService.GetActiveUserbyId(requestedUserId);

                var user = new SendNewuser
                {
                    ChatRoomId = roomDetail.RoomId,
                    ChatRoomName = requestedUserId.ToString(),
                    RequestedUserId = requestedUserId,
                    RequestedUsername = requestedUsername,
                    RequestedUserImageUrl = requestedUserImageUrl,
                    RoomType = 1,
                    UserStatus = userStatus.UserStatus, // Active for Online
                    CreatedDate = nowForInitiatedUser,
                    LastMessageDetails = new LastMessageDetails
                    {
                        RoomId = roomDetail.RoomId,
                        UserId = initiatedUserid,
                        Message = $"{requestedUsername} has joined the Chat.",
                        CreatedDate = nowForInitiatedUser.ToString("yyyy-MM-ddTHH:mm:ss")
                    }
                };
                ConnectedUsers.Add(user);

                var reqUserConnectionId = await _userService.GetConnectionIdByuserId(requestedUserId);

                var InitiatedUsername = await _userService.GetUserNamebyId(initiatedUserid);

                var InitiatedUserImageUrl = await _userService.GetUserPicturebyId(initiatedUserid);

                if (reqUserConnectionId != null)
                {
                    await Groups.AddToGroupAsync(reqUserConnectionId, roomID.ToString());

                    var RequestedUserInfo = await _userService.IsUserExist(requestedUserId);
                    var requestedLocalTime = await _timezoneConverterService
                    .GetCurrentTimeByZoneId(DateTime.UtcNow, (int)RequestedUserInfo.TimeZoneId);


                    /*Send new room details to the Requested User*/

                    var clientuser = new SendNewuser
                    {
                        ChatRoomId = roomDetail.RoomId,
                        ChatRoomName = requestedUserId.ToString(),
                        RequestedUserId = initiatedUserid,
                        RequestedUsername = InitiatedUsername,
                        RequestedUserImageUrl = InitiatedUserImageUrl,
                        RoomType = 1,
                        UserStatus = userStatus.UserStatus, // Active for Online
                        CreatedDate = requestedLocalTime,
                        LastMessageDetails = new LastMessageDetails
                        {
                            RoomId = roomDetail.RoomId,
                            UserId = initiatedUserid,
                            Message = $"{InitiatedUsername} has joined the Chat.",
                            CreatedDate = requestedLocalTime.ToString("yyyy-MM-ddTHH:mm:ss")
                        }
                    };

                    await Clients.Client(reqUserConnectionId).SendAsync("NewUser", clientuser);

                    var messages = new List<Messages>();
                    messages.Add(new Messages
                    {
                        UserId = initiatedUserid,
                        Message = $"{InitiatedUsername} has joined the Chat.",
                        CreatedDate = requestedLocalTime.ToString("yyyy-MM-ddTHH:mm:ss")
                    });

                    var messageResponse = new MessageResponse
                    {
                        RoomId = roomDetail.RoomId,
                        RoomName = "",
                        Messages = messages
                    };

                    await Clients.Client(reqUserConnectionId).SendAsync("ReceiveMessage", messageResponse);

                }

                var InitiatedUserInfo = await _userService.IsUserExist(initiatedUserid);

                var createdDateForInitiator = await _timezoneConverterService
                    .GetCurrentTimeByZoneId(DateTime.UtcNow, (int)InitiatedUserInfo.TimeZoneId);

                var chatMessage = new ChatMessages
                {
                    RoomId = roomDetail.RoomId,
                    UserId = initiatedUserid,
                    Message = $"{InitiatedUsername} has joined the Chat.",
                    Status = 0,
                    CreatedDate = DateTime.UtcNow
                };
                await _chatroomService.AddMessages(chatMessage);

                await Clients.Client(Context.ConnectionId).SendAsync("NewUser", user);

                return new DTORoomResponse.DTORoomCreatedResponse { Message = "Room Created Successfully..", RoomId = roomDetail.RoomId };
            }
        }

        // CreateGroup -- Which is used to Create a Group with Multiple user's by their userId's, When invoked in client side(Group Chat) .
        public async Task<DTORoomResponse.DTORoomCreatedResponse> CreateGroup(string roomName, int? batchId, List<int> userids)
        {
            int userId = int.Parse(Context.User.FindFirst("id").Value);

            if (roomName != null && userids != null)
            {
                Guid roomID = Guid.NewGuid();

                var utcNow = DateTime.UtcNow;

                var roomDetail = new ChatRoom
                {
                    RoomId = roomID.ToString(),
                    BatchId = (batchId != null || batchId != 0) ? batchId : 0,
                    RoomName = roomName,
                    CreatedDate = utcNow
                };

                await _chatroomService.AddChatRoom(roomDetail);

                foreach (var uId in userids)
                {
                    // Fetching ConnectionId from the ActiveUsers table.
                    var userConnectionId = await _userService.GetConnectionIdByuserId(uId);

                    // Convert UTC to user's local time for socket
                    var userInfo = await _userService.IsUserExist(uId);
                    var localTime = await _timezoneConverterService.GetCurrentTimeByZoneId(utcNow, (int)userInfo.TimeZoneId);

                    if (userConnectionId != null)
                    {
                        await Groups.AddToGroupAsync(userConnectionId, roomID.ToString());

                        /*Send new group details to the Requested User*/

                        var clientGroup = new SendNewGroup
                        {
                            ChatRoomId = roomDetail.RoomId,
                            ChatRoomName = roomName,
                            GroupUsers = null,
                            RoomType = 2,
                            CreatedDate = localTime
                        };

                        await Clients.Client(userConnectionId).SendAsync("NewGroup", clientGroup);

                        var messages = new List<Messages>
                        {
                            new Messages
                            {
                                UserId = userId,
                                Message = "Welcome to the Group.",
                                CreatedDate = localTime.ToString("yyyy-MM-ddTHH:mm:ss") // ✅ user local time
                            }
                        };

                        var messageResponse = new MessageResponse
                        {
                            RoomId = roomDetail.RoomId,
                            RoomName = roomName,
                            Messages = messages
                        };

                        await Clients.Client(userConnectionId).SendAsync("ReceiveMessage", messageResponse);

                    }

                    var roomMapping = new ChatRoomMapping
                    {
                        RoomId = roomID.ToString(),
                        UserId = uId,
                        RoomType = 2,
                        Status = 1,
                        CreatedDate = utcNow // ✅ UTC
                    };

                    await _chatroomService.ChatRoomMap(roomMapping);

                }

                var chatMessage = new ChatMessages
                {
                    RoomId = roomDetail.RoomId,
                    UserId = userId,
                    Message = "Welcome to the Group.",
                    Status = 0,
                    CreatedDate = utcNow // ✅ UTC
                };
                await _chatroomService.AddMessages(chatMessage);

                return new DTORoomResponse.DTORoomCreatedResponse { Message = "Group Created Successfully..", RoomId = roomDetail.RoomId };

            }
            return new DTORoomResponse.DTORoomCreatedResponse { Message = "Invalid Input", RoomId = "" };

        }

        // AddNewUsertoGroup -- which is used for Add New user to the Group by the userid, invoked from client side(Group chat).
        public async Task<DTORoomResponse.DTORoomCreatedResponse> AddNewUsertoGroup(string roomId, string roomName, int userid)
        {
            int ContextuserId = int.Parse(Context.User.FindFirst("id").Value);

            var addtoRoom = await _chatroomService.AddNewUserToGroup(roomId, roomName, userid, ContextuserId);

            if (addtoRoom > 0)
            {
                return new DTORoomCreatedResponse { Message = "User Added Successfully..", RoomId = roomId };
            }
            return new DTORoomCreatedResponse { Message = addtoRoom == -1 ? "User already exist in Group.." : "Something went wrong", RoomId = roomId };
        }

        // RemoveUserFromGroup -- Used to remove the particular user from the group.
        public async Task<DTORoomCreatedResponse> RemoveUserFromGroup(string roomId, int userid)
        {
            int contextUserId = int.Parse(Context.User.FindFirst("id").Value);

            var remove = await _chatroomService.RemoveUserFromGroup(roomId, userid, contextUserId);

            if (remove > 0)
            {
                return new DTORoomCreatedResponse { Message = "User Removed Successfully..", RoomId = roomId };
            }
            return new DTORoomCreatedResponse { Message = "Something went wrong", RoomId = null };
        }

        public async Task DeleteGroup(string roomId)
        {
            /* Need to implement */
        }

        // Method will be invoked in client side For Message sending purpose For both Room & Group.
        public async Task<MessageResponse> SendMessage(string roomId, int senderId, string message)
        {
            var SenderDetails = await _userService.GetUserDetailsById(senderId);

            var roomDetails = await _chatroomService.GetChatRoomByRoomId(roomId);

            var SenderInfo = await _userService.IsUserExist(senderId);

            var utcNow = DateTime.UtcNow;

            var chatMessage = new ChatMessages
            {
                RoomId = roomId,
                UserId = senderId,
                Message = message,
                Status = 0,
                IsEdited = false,
                CreatedDate = utcNow   // ✅ Store UTC in database
            };

            chatMessage = await _chatroomService.AddMessages(chatMessage);

            int unreadCount = await _chatroomService.GetUnreadCount(roomId, senderId);

            // **Convert UTC to sender's timezone for socket**
            var localTime = await _timezoneConverterService
                .GetCurrentTimeByZoneId(utcNow, (int)SenderInfo.TimeZoneId);

            var messages = new List<Messages>
            {
                new Messages
                {
                  MessageId = chatMessage.Id,
                  UserId = senderId,
                  UserName = SenderDetails.UserName,
                  UserImage = SenderDetails.ProfilePicture,
                  Message = message ,
                  IsEdited = false,
                  UnreadMessageCount = unreadCount,
                  CreatedDate = localTime.ToString("yyyy-MM-ddTHH:mm:ss") // ✅ Send local time
                }};

            var messageResponse = new MessageResponse
            {
                RoomId = roomId,
                RoomName = "",
                Messages = messages
            };


            await Clients.Group(roomId).SendAsync("ReceiveChatMessage", messageResponse);

            // Optional push notification
            if (_appSettings.EnablePushNotificationForChat)
            {
                var title = roomDetails.BatchId != 0 ? roomDetails.RoomName : SenderDetails.UserName;
                var toMessage = roomDetails.BatchId != 0 ? $"{SenderDetails.UserName}: {message}" : message;

                await _chatroomService.SendPushNotification(roomId, senderId, title, toMessage);
            }
            return messageResponse;
        }

        // AddtoActiveUsers -- This method used in side OnConnectedAsync , it will used to Add (or) update the connectionId in the Active users table.
        public async Task AddtoActiveUsers(int Userid)
        {
            ActiveUser userexist = await _userService.GetActiveUserbyId(Userid);

            if (userexist != null)
            {
                userexist.ConnectionId = Context.ConnectionId;
                userexist.UserStatus = 1;
                userexist.LastOnline = DateTime.Now;
                await _userService.UpdateActiveUser(userexist);

            }

            else
            {
                var addtoActiveUsers = new ActiveUser
                {
                    UserId = Userid,
                    ConnectionId = Context.ConnectionId,
                    UserStatus = 1,
                    LastOnline = DateTime.Now
                };

                await _userService.AddActiveUser(addtoActiveUsers);
            }

        }

        // LeaveFromActiveUsers -- This method is used inside the OnDisconnectedAsync method , and this method is used to update the user status as offline in Active users table.
        public async Task LeaveFromActiveUsers(int Userid)
        {
            ActiveUser userexist = await _userService.GetActiveUserbyId(Userid);
            userexist.UserStatus = 0;
            userexist.LastOnline = DateTime.Now;
            await _userService.UpdateActiveUser(userexist);

        }

        // AddtoGroup -- used to add Our ConnectionId into the particular Room (or) Group.
        public async Task AddtoGroup(string roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

        }

        /* OnConnectedAsync -- This method is automatically invoked, when the WebSocket is Established. 
                               In this state it can Add our Connection to all Existing Groups and Rooms. */
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine("OnConnectedAsync");

            int userId = int.Parse(Context.User.FindFirst("id").Value);

            var username = await _userService.GetUserNamebyId(userId);

            await AddtoActiveUsers(userId);

            var roomids = await _chatroomService.GetAllRoomIdByUserId(userId);

            if (roomids != null)
            {
                foreach (var roomId in roomids)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
                    await Clients.Group(roomId).SendAsync("OnConnected", userId, $"{username} is online.");
                }
            }

            await base.OnConnectedAsync();
        }

        /* OnDisconnectedAsync -- This method is automatically invoked, when the WebSocket is Aborted by the client,
                                  In this state it can remove our Connection from all Groups and Rooms.*/
        public override async Task OnDisconnectedAsync(Exception exception)
        {

            Console.WriteLine("OnDisconnectedAsync");

            int userId = int.Parse(Context.User.FindFirst("id").Value);

            var username = await _userService.GetUserNamebyId(userId);

            await LeaveFromActiveUsers(userId);

            var roomids = await _chatroomService.GetAllRoomIdByUserId(userId);

            if (roomids != null)
            {
                foreach (var roomId in roomids)
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
                    await Clients.Group(roomId).SendAsync("OnDisconnected", userId, $"{username} is offline.", DateTime.Now);
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task StartTyping(string roomId, int userId)
        {
            var username = await _userService.GetUserNamebyId(userId);

            var messagetypingResponse = new MessageTyping
            {
                UserId = userId,
                RoomId = roomId,
                Message = $"{username} is typing"
            };

            await Clients.Group(roomId).SendAsync("UserStartedTyping", messagetypingResponse);
        }

        public async Task StopTyping(string roomId, int userId)
        {
            var username = await _userService.GetUserNamebyId(userId);
            await Clients.Group(roomId).SendAsync("UserStoppedTyping", userId);
        }

        public async Task<MessageResponse> UpdateMessage(int messageId, string newMessage, int userId)
        {
            var existingMsg = await _chatroomService.GetMessageById(messageId);

            if (existingMsg == null)
                return new MessageResponse { RoomId = "", Messages = null };

            if (existingMsg.UserId != userId)
                return new MessageResponse { RoomId = existingMsg.RoomId, Messages = null }; // Not allowed


            // Update DB
            existingMsg.Message = newMessage;
            existingMsg.UpdatedDate = DateTime.UtcNow;
            existingMsg.IsEdited = true;

            await _chatroomService.UpdateMessage(existingMsg);

            // Convert UTC to user's timezone (same as SendMessage)
            var senderInfo = await _userService.IsUserExist(userId);
            var localTime = await _timezoneConverterService
                    .GetCurrentTimeByZoneId(existingMsg.UpdatedDate.Value, (int)senderInfo.TimeZoneId);

            var senderDetails = await _userService.GetUserDetailsById(userId);
            int unreadCount = await _chatroomService.GetUnreadCount(existingMsg.RoomId, userId);

            // Build response for client
            var responseMessage = new Messages
            {
                MessageId = existingMsg.Id,
                UserId = existingMsg.UserId,
                UserName = senderDetails.UserName,
                UserImage = senderDetails.ProfilePicture,
                Message = newMessage,
                UnreadMessageCount = unreadCount,
                CreatedDate = localTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                IsEdited = true
            };

            var messageResponse = new MessageResponse
            {
                RoomId = existingMsg.RoomId,
                RoomName = "",
                Messages = new List<Messages> { responseMessage }
            };

            // Broadcast to all members in the room
            await Clients.Group(existingMsg.RoomId).SendAsync("MessageUpdated", messageResponse);

            return messageResponse;
        }

        public async Task<MessageResponse> DeleteMessage(int messageId, int userId)
        {
            var existingMsg = await _chatroomService.GetMessageById(messageId);

            if (existingMsg == null)
                return new MessageResponse { RoomId = "", Messages = null };

            if (existingMsg.UserId != userId)
                return new MessageResponse { RoomId = existingMsg.RoomId, Messages = null };

            var roomId = existingMsg.RoomId;

            await _chatroomService.DeleteMessage(existingMsg);


            var responseMessage = new Messages
            {
                MessageId = messageId,
                UserId = userId
            };

            var messageResponse = new MessageResponse
            {
                RoomId = roomId,
                Messages = new List<Messages> { responseMessage }
            };

            // 5️⃣ Notify all clients in the room
            await Clients.Group(roomId).SendAsync("MessageDeleted", messageResponse);

            return messageResponse;
        }
    }

}


