using System.Collections;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using acepickle_api.Helper;
using acepickle_chat_api.Helper;
using acepickle_chat_api.Models;
using acepickle_chat_api.Service;
using static acepickle_chat_api.Dto.DTORoomResponse;

namespace acepickle_chat_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IChatRoomService _chatroomService;
        public ChatController(IChatRoomService chatRoomService)
        {
            _chatroomService = chatRoomService;
        }

        [HttpGet("GetAllRoomsByUserId")]
        public async Task<IEnumerable<object>> GetAllRoomsByUserId(string? searchKeyword)
        {
            var userId = await Utils.GetUserIDByCliam(HttpContext);

            return await _chatroomService.GetAllRoomsByUserId(userId, searchKeyword);
        }

        [HttpGet("GetAllGroupsByUserId")]
        public async Task<IEnumerable> GetAllGroupsByUserId(int? batchId, string? searchKeyword)
        {
            var userId = await Utils.GetUserIDByCliam(HttpContext);

            return await _chatroomService.GetAllGroupsByUserId(userId, batchId, searchKeyword);
        }

        [HttpGet("GetAllMessagesByRoomId")]
        public async Task<MessageResponse> GetAllMessagesByRoomId(string roomId)
        {
            int contextUserId = int.Parse(User.FindFirst("id").Value);

            return await _chatroomService.GetAllMessages(roomId, contextUserId);
        }

        [HttpGet("GetAllUsersByRoomId")]
        public async Task<IEnumerable> GetAllUsersByRoomId(string groupId)
        {
            return await _chatroomService.GetAllUsersByRoomId(groupId);
        }

        [HttpPut("UpdateChatRoomById/{roomid}")]
        public async Task<ActionResult> UpdateChatRoomById(string roomid, int batchId, string roomName)
        {
            ChatRoom _result = await _chatroomService.GetChatRoomByRoomId(roomid);

            if (_result != null)
            {
                _result.BatchId = batchId;
                _result.RoomName = roomName;

                await _chatroomService.UpdateChatRoom(_result);

                return Ok(new DTOCommonResponse { Status = true, Message = Constants.RecordUpdated });
            }
            return Ok(new DTOCommonResponse { Status = false, Message = Constants.InvalidInput });
        }

        [HttpDelete("RemoveUserFromGroup/{roomid}")]
        public async Task<DTORoomCreatedResponse> RemoveUserFromGroup(string roomid, int userid)
        {
            int contextUserId = int.Parse(User.FindFirst("id").Value);

            var remove = await _chatroomService.RemoveUserFromGroup(roomid, userid, contextUserId);

            if (remove > 0)
            {
                return new DTORoomCreatedResponse { Message = "User Removed Successfully..", RoomId = roomid };
            }
            return new DTORoomCreatedResponse { Message = "Something went wrong", RoomId = null };
        }

        [HttpPost("AddNewUserToGroup/{roomid}")]
        public async Task<DTORoomCreatedResponse> AddNewUserToGroup(string roomid, string roomName, int userid)
        {
            int ContextuserId = int.Parse(User.FindFirst("id").Value);

            var addtoRoom = await _chatroomService.AddNewUserToGroup(roomid, roomName, userid, ContextuserId);

            if (addtoRoom > 0)
            {
                return new DTORoomCreatedResponse { Message = "User Added Successfully..", RoomId = roomid };
            }

            return new DTORoomCreatedResponse { Message = addtoRoom == -1 ? "User already exist in Group.." : "Something went wrong", RoomId = roomid };
        }

        [HttpPut("ReadAllMessages/{roomid}")]
        public async Task<DTOCommonResponse> ReadAllMessages(string roomid)
        {
            int ContextuserId = int.Parse(User.FindFirst("id").Value);

            var addtoRoom = await _chatroomService.ReadAllMessages(roomid, ContextuserId);

            if (addtoRoom > 0)
            {
                return new DTOCommonResponse { Status = true, Message = Constants.RecordUpdated };
            }
            return new DTOCommonResponse { Status = false, Message = Constants.InvalidInput };
        }

    }
}