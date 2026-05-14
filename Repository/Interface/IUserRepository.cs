using softskiller_chat_api.Models;
using static softskiller_chat_api.Dto.DTORoomResponse;

namespace softskiller_chat_api.Repository
{
    public interface IUserRepository
    {
        Task<string> GetUserNamebyId(int userId);  
        Task<string> GetUserPicturebyId(int userId);  
        Task<(string UserName, string ProfilePicture)> GetUserDetailsById(int userId);

        // ActiveUser Repository Related
        Task<List<ActiveUser>> GetAllActiveUser();
        Task<int> AddActiveUser(ActiveUser activeUser);
        Task<int> UpdateActiveUser(ActiveUser activeUser);
        Task<int> isExist(int userid);
        Task<User> IsUserExist(int userid);
        Task<string> GetConnectionIdByuserId(int userid);
        Task<ActiveUser> GetActiveUserbyId(int userId);
        Task<int> RemoveActiveUser(int id);
    }
}