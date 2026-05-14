using softskiller_chat_api.Models;
using softskiller_chat_api.Repository;
using static softskiller_chat_api.Dto.DTORoomResponse;

namespace softskiller_chat_api.Service
{
    public class UserService : IUserService
    {   
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<string> GetUserNamebyId(int userId)
        {
            return await _userRepository.GetUserNamebyId(userId);
        }

        public async Task<string> GetUserPicturebyId(int userId)
        {
            return await _userRepository.GetUserPicturebyId(userId);
        }
        public async Task<(string UserName, string ProfilePicture)> GetUserDetailsById(int userId)
        {
            return await _userRepository.GetUserDetailsById(userId);
        }

        // ActiveUser Service Related
        public async Task<int> AddActiveUser(ActiveUser activeUser)
        {
             return await _userRepository.AddActiveUser(activeUser);
        }

        public async Task<int> UpdateActiveUser(ActiveUser activeUser)
        {
            return await _userRepository.UpdateActiveUser(activeUser);
        }

        public async Task<List<ActiveUser>> GetAllActiveUser()
        {
            return await _userRepository.GetAllActiveUser();
        }

        public async Task<int> isExist(int userid)
        {
            return await _userRepository.isExist(userid);
        }
        public async Task<User> IsUserExist(int userid)
        {
            return await _userRepository.IsUserExist(userid);
        }
        
        public async Task<string> GetConnectionIdByuserId(int userid)
        {
            return await _userRepository.GetConnectionIdByuserId(userid);
        }
        public async Task<ActiveUser> GetActiveUserbyId(int userId)
        {
            return await _userRepository.GetActiveUserbyId(userId);
        }

        public async Task<int> RemoveActiveUser(int id)
        {
            return await _userRepository.RemoveActiveUser(id);
        }
    }

}      