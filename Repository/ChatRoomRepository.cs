using System.Collections;
using Microsoft.EntityFrameworkCore;
using softskiller_chat_api.Data;
using softskiller_chat_api.Helper;
using softskiller_chat_api.Models;
using static softskiller_chat_api.Dto.DTORoomResponse;

namespace softskiller_chat_api.Repository
{
    public class ChatRoomRepository : IChatRoomRepository
    {
        private readonly MyDBContext _dbContext;
        private readonly TimeZoneConverter _timezoneConverterService;

        public ChatRoomRepository(MyDBContext dbContext, TimeZoneConverter timezoneConverterService)
        {
            _dbContext = dbContext;
            _timezoneConverterService = timezoneConverterService;
        }
        public async Task<int> AddChatRoom(ChatRoom chatRoom)
        {
            _dbContext.ChatRooms.Add(chatRoom);
            return await Save();
        }
        public async Task<int> UpdateChatRoom(ChatRoom chatRoom)
        {
            _dbContext.ChatRooms.Update(chatRoom);
            return await Save();
        }

        public async Task<IEnumerable<object>> GetAllRoomsByUserId(int id, string searchKeyword)
        {
            var myUser = await _dbContext.Users
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync();

            var myUserImage = await _dbContext.UserDetails
                .Where(ud => ud.UserId == id)
                .Select(ud => ud.ProfilePicture)
                .FirstOrDefaultAsync();

            var rooms = await (
                from crMapping in _dbContext.ChatRoomMappings
                join chatroom in _dbContext.ChatRooms on crMapping.RoomId equals chatroom.RoomId
                join otherMapping in _dbContext.ChatRoomMappings on crMapping.RoomId equals otherMapping.RoomId
                join otherUser in _dbContext.Users on otherMapping.UserId equals otherUser.Id
                where crMapping.UserId == id
                      && otherMapping.UserId != id
                      && crMapping.RoomType == 1
                      && (string.IsNullOrEmpty(searchKeyword) || otherUser.Name.Contains(searchKeyword))
                select new
                {
                    ChatRoomId = chatroom.RoomId,
                    ChatRoomName = chatroom.RoomName,
                    MyUserId = id,
                    MyUserName = myUser.Name,
                    MyImageUrl = myUserImage,
                    RequestedUserId = otherUser.Id,
                    RequestedUsername = otherUser.Name,
                    RequestedUserImageUrl = _dbContext.UserDetails
                        .Where(ud => ud.UserId == otherUser.Id)
                        .Select(ud => ud.ProfilePicture)
                        .FirstOrDefault(),
                    RoomType = crMapping.RoomType,
                    UserStatus = _dbContext.ActiveUsers
                        .Where(au => au.UserId == otherUser.Id)
                        .Select(au => au.UserStatus)
                        .FirstOrDefault(),
                    CreatedDate = crMapping.CreatedDate
                }
            ).ToListAsync();

            var roomIds = rooms.Select(r => r.ChatRoomId).ToList();

            // Get last messages for each room using a different approach
            var lastMessages = await _dbContext.ChatMessages
                .Where(m => roomIds.Contains(m.RoomId))
                .GroupBy(m => m.RoomId)
                .Select(g => new
                {
                    RoomId = g.Key,
                    LastMessageId = g.Max(m => m.Id) // Get the highest ID instead of using OrderBy + FirstOrDefault
                })
                .ToListAsync();

            // Get the actual message details using the IDs we found
            var lastMessageIds = lastMessages.Select(lm => lm.LastMessageId).ToList();
            var messageDetails = await _dbContext.ChatMessages
                .Where(m => lastMessageIds.Contains(m.Id))
                .ToListAsync();

            // Get unread counts separately
            var unreadCounts = await _dbContext.ChatMessages
                .Where(m => roomIds.Contains(m.RoomId) && m.Status == 0 && m.UserId != id)
                .GroupBy(m => m.RoomId)
                .Select(g => new
                {
                    RoomId = g.Key,
                    UnreadCount = g.Count()
                })
                .ToListAsync();

            var resultList = new List<object>();

            foreach (var r in rooms)
            {
                var lastMsg = lastMessages.FirstOrDefault(lm => lm.RoomId == r.ChatRoomId);
                var msgDetail = lastMsg != null ? messageDetails.FirstOrDefault(md => md.Id == lastMsg.LastMessageId) : null;
                var unreadData = unreadCounts.FirstOrDefault(uc => uc.RoomId == r.ChatRoomId);
            
                var convertedRoomCreatedDate = (await _timezoneConverterService
                    .GetCurrentTimeByZoneId(r.CreatedDate, (int)myUser.TimeZoneId))
                    .ToString("yyyy-MM-ddTHH:mm:ss");
            
                string? convertedLastMessageDate = null;
                if (msgDetail != null)
                {
                    convertedLastMessageDate = (await _timezoneConverterService
                        .GetCurrentTimeByZoneId(msgDetail.CreatedDate, (int)myUser.TimeZoneId))
                        .ToString("yyyy-MM-ddTHH:mm:ss");
                }
            
                resultList.Add(new
                {
                    r.ChatRoomId,
                    r.ChatRoomName,
                    r.MyUserId,
                    r.MyUserName,
                    r.MyImageUrl,
                    r.RequestedUserId,
                    r.RequestedUsername,
                    r.RequestedUserImageUrl,
                    r.RoomType,
                    r.UserStatus,
                    CreatedDate = convertedRoomCreatedDate,
                    LastMessageDetails = msgDetail == null ? null : new
                    {
                        msgDetail.RoomId,
                        msgDetail.UserId,
                        msgDetail.Message,
                        CreatedDate = convertedLastMessageDate,
                        UnreadMessageCount = unreadData?.UnreadCount ?? 0
                    },
                    LastMessagedDate = msgDetail?.CreatedDate
                });
            }
            
            // Then order by LastMessagedDate
            var finalResult = resultList
                .OrderByDescending(r => r.GetType().GetProperty("LastMessagedDate")?.GetValue(r) ?? DateTime.MinValue)
                .Select(r => r);
            
            return finalResult;
        }


        public async Task<IEnumerable> GetAllGroupsByUserId(int userid, int? batchId, string? searchkeyword)
        {
            var myUser = await _dbContext.Users.Where(u => u.Id == userid).FirstOrDefaultAsync();
            var Result = await (
                from crMapping in _dbContext.ChatRoomMappings
                join chatroom in _dbContext.ChatRooms on crMapping.RoomId equals chatroom.RoomId
                where crMapping.UserId == userid && crMapping.RoomType == 2 && crMapping.Status == 1 &&
                      (chatroom.RoomName.Contains(searchkeyword) || searchkeyword == null) && (batchId == null || chatroom.BatchId == batchId)
                orderby chatroom.Id ascending
                select new
                {
                    ChatRoomId = chatroom.RoomId,
                    ChatRoomName = chatroom.RoomName,
                    GroupUsers = (
                                   from otherMapping in _dbContext.ChatRoomMappings
                                   join otherUser in _dbContext.Users on otherMapping.UserId equals otherUser.Id
                                   join userDetails in _dbContext.UserDetails on otherUser.Id equals userDetails.UserId
                                   where otherMapping.RoomId == chatroom.RoomId && otherMapping.Status != 0
                                   select new
                                   {
                                       UserId = otherUser.Id,
                                       UserName = otherUser.Name,
                                       JoinedUserImage = userDetails.ProfilePicture
                                   }
                    ).ToList(),
                    RoomType = crMapping.RoomType,
                    CreatedDate = crMapping.CreatedDate
                }
            ).ToListAsync();
            var connectionDetails = Result.Select(r => new
            {
                r.ChatRoomId,
                r.ChatRoomName,
                r.GroupUsers,
                r.RoomType,
                CreatedDate = _timezoneConverterService
            .GetCurrentTimeByZoneId(r.CreatedDate, (int)myUser.TimeZoneId)
            .Result.ToString("yyyy-MM-ddTHH:mm:ss")
            });


            return connectionDetails;
        }



        public async Task<ChatRoom> GetChatRoomById(int id)
        {
            return await _dbContext.ChatRooms.FindAsync(id);
        }
        public async Task<ChatRoom> GetChatRoomByRoomId(string roomid)
        {
            return await _dbContext.ChatRooms.FirstOrDefaultAsync(x => x.RoomId == roomid);
        }

        public async Task<bool> isChatRoomExist(string roomName)
        {
            return await _dbContext.ChatRooms.AnyAsync(x => x.RoomName == roomName);
        }

        //ChatMessage
        public async Task<ChatMessages> AddMessages(ChatMessages chatObject)
        {      
            _dbContext.ChatMessages.Add(chatObject);
            await _dbContext.SaveChangesAsync();
        
            return chatObject;
        }

        public async Task<int> UpdateMessages(List<ChatMessages> chatMessage)
        {
            _dbContext.ChatMessages.UpdateRange(chatMessage);
            return await Save();
        }

        // public async Task<MessageResponse> GetAllMessages(string roomId)
        // {
        //     var messages = (from cr in _dbContext.ChatRooms
        //                     where cr.RoomId == roomId
        //                     select new MessageResponse
        //                     {
        //                         RoomId = cr.RoomId,
        //                         RoomName = cr.RoomName,
        //                         Messages = (from cm in _dbContext.ChatMessages
        //                                     join userDetails in _dbContext.Users on cm.UserId equals userDetails.Id
        //                                     where cm.RoomId == cr.RoomId
        //                                     orderby cm.Id
        //                                     select new Messages
        //                                     {
        //                                         UserId = cm.UserId,
        //                                         UserName = userDetails.Name,
        //                                         UserImage = _dbContext.UserDetails
        //                                                         .Where(ud => ud.UserId == cm.UserId)
        //                                                         .Select(ud => ud.ProfilePicture)
        //                                                         .FirstOrDefault() ?? null,
        //                                         Message = cm.Message,
        //                                         CreatedDate = cm.CreatedDate.ToString("yyyy-MM-ddTHH:mm:ss")
        //                                     }).ToList()
        //                     });

        //     MessageResponse singleMessageResponse = messages.FirstOrDefault();

        //     return singleMessageResponse;
        // }

        /*This is a version 1 method, above code is a old method. Need to remove old method after validation*/
        public async Task<MessageResponse> GetAllMessages(string roomId, int userId)
        {
            var userInfo = await _dbContext.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
            
            var room = await _dbContext.ChatRooms
                .Where(cr => cr.RoomId == roomId)
                .FirstOrDefaultAsync();

            if (room == null) return null;

            var chatMessages = await (
                from cm in _dbContext.ChatMessages
                join userDetails in _dbContext.Users on cm.UserId equals userDetails.Id
                where cm.RoomId == roomId
                orderby cm.Id
                select new
                {
                    cm.Id,
                    cm.UserId,
                    UserName = userDetails.Name,
                    UserImage = _dbContext.UserDetails
                        .Where(ud => ud.UserId == cm.UserId)
                        .Select(ud => ud.ProfilePicture)
                        .FirstOrDefault(),
                    cm.Message,
                    cm.CreatedDate,
                    cm.IsEdited
                }
            ).ToListAsync();
        
            // Convert all messages to the user's timezone
            var messages = new List<Messages>();
            foreach (var m in chatMessages)
            {
                // ⭐ Convert to user's timezone
                var dt = await _timezoneConverterService
                    .GetCurrentTimeByZoneId(m.CreatedDate, (int)userInfo.TimeZoneId);
        
            messages.Add(new Messages
            {
                MessageId = m.Id,
                UserId = m.UserId,
                UserName = m.UserName,
                UserImage = m.UserImage,
                Message = m.Message,
                IsEdited = m.IsEdited,
                CreatedDate = dt.ToString("yyyy-MM-ddTHH:mm:ss") // FINAL OUTPUT
            });
            }
        
            return new MessageResponse
            {
                RoomId = room.RoomId,
                RoomName = room.RoomName,
                Messages = messages
            };
        }

        //ChatRoom Mapping
        public async Task<int> ChatRoomMap(ChatRoomMapping chatRoomMapping)
        {
            _dbContext.ChatRoomMappings.Add(chatRoomMapping);
            return await Save();
        }

        public async Task<List<string>> GetAllRoomIdByUserId(int userid)
        {
            return await _dbContext.ChatRoomMappings.Where(x => x.UserId == userid && x.Status != 0).Select(x => x.RoomId).ToListAsync();
        }

        public async Task<IEnumerable> GetAllUsersByRoomId(string groupId)
        {
            var userList = (from crMapping in _dbContext.ChatRoomMappings
                            join user in _dbContext.Users on crMapping.UserId equals user.Id
                            where crMapping.RoomId == groupId && crMapping.Status == 1
                            orderby user.Name ascending
                            select new
                            {
                                UserId = user.Id,
                                UserName = user.Name,
                                ImageUrl = (from ud in _dbContext.UserDetails where ud.UserId == user.Id select ud.ProfilePicture).FirstOrDefault(),
                            }).ToList();

            return userList;
        }

        public string RoomExist(int initiatedUserId, int joinedUserid)
        {
            // return _dbContext.ChatRoomMappings.Where(x =>( x.UserId == initiatedUserId )&&( x.UserId == joinedUserid )).Select( x => x.RoomId ).FirstOrDefault(); 
            var roomIds = _dbContext.ChatRoomMappings
                          .Where(x => (x.UserId == initiatedUserId || x.UserId == joinedUserid) && x.RoomType == 1)
                          .GroupBy(x => x.RoomId)
                          .Where(g => g.Count() == 2)
                          .Select(g => g.Key)
                          .FirstOrDefault();

            return roomIds;
        }

        public async Task<int> DeActivateUserFromGroup(string roomId, int userId)
        {
            var mappingDetail = _dbContext.ChatRoomMappings.Where(x => x.UserId == userId && x.RoomId == roomId).Select(x => x).FirstOrDefault();
            mappingDetail.Status = 0;
            _dbContext.ChatRoomMappings.Update(mappingDetail);
            return await Save();
        }

        public async Task<ChatRoomMapping> IsUserExistinRoom(string roomId, int userId)
        {
            return _dbContext.ChatRoomMappings.Where(x => x.UserId == userId && x.RoomId == roomId).Select(x => x).FirstOrDefault();
        }

        public async Task<int> UpdateChatRoomMapping(ChatRoomMapping chatRoomMapping)
        {
            _dbContext.ChatRoomMappings.Update(chatRoomMapping);
            return await Save();
        }

        public async Task<int> GetUnreadCount(string roomId, int senderId)
        {
            return await _dbContext.ChatMessages
                .Where(m => m.RoomId == roomId && m.UserId == senderId && m.Status == 0)
                .CountAsync();
        }
        public async Task<List<ChatMessages>> GetUnreadMessages(string roomId, int senderId)
        {
            return await _dbContext.ChatMessages
                .Where(m => m.RoomId == roomId && m.UserId != senderId && m.Status == 0)
                .ToListAsync();
        }

        public async Task<List<int>> GetActiveUsersByRoomId(string roomId, int currentUserId)
        {
            var now = DateTime.UtcNow;

            var activeUsers = await (
                from chat in _dbContext.ChatRoomMappings
                join course in _dbContext.CourseMappings 
                    on chat.UserId equals course.Userid
                where chat.RoomId == roomId
                      && chat.UserId != currentUserId
                      && chat.Status == 1
                      && course.Status == 1
                      && course.Validity > now        
                select chat.UserId
            )
            .Distinct()
            .ToListAsync();

            return activeUsers;
        }
        public async Task<ChatMessages> GetMessageById(int messageId)
        {
            return await _dbContext.ChatMessages
                .FirstOrDefaultAsync(x => x.Id == messageId);
        }

        public async Task<int> UpdateMessage(ChatMessages message)
        {
            _dbContext.ChatMessages.Update(message);
            return await Save();
        }
        public async Task<int> DeleteMessage(ChatMessages entity)
        {
            _dbContext.ChatMessages.Remove(entity);
            return await Save();
        }

        public async Task<RoomMappingDto> GetRoomMappingByRoomId(string roomId)
        {
            return await (from cm in _dbContext.ChatRoomMappings
                          join cr in _dbContext.ChatRooms on cm.RoomId equals cr.RoomId into crGroup
                          from cr in crGroup.DefaultIfEmpty()
                          where cm.RoomId == roomId
                          select new RoomMappingDto
                          {
                              RoomType = cm.RoomType,
                              BatchId = cr != null ? cr.BatchId : 0
                          })
                         .FirstOrDefaultAsync();
        }

        private async Task<int> Save()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}