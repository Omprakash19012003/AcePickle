using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using softskiller_chat_api.Data;
using softskiller_chat_api.Models;
using static softskiller_chat_api.Dto.DTORoomResponse;

namespace softskiller_chat_api.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly MyDBContext _dbContext;
        public UserRepository(MyDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> GetUserNamebyId(int userId)
        {
            return await _dbContext.Users.Where(x => x.Id == userId).Select(x => x.Name).FirstOrDefaultAsync();
        }
        public async Task<(string UserName, string ProfilePicture)> GetUserDetailsById(int userId)
        {
            var result = await (from user in _dbContext.Users
                                join userDetails in _dbContext.UserDetails
                                on user.Id equals userDetails.UserId
                                where user.Id == userId
                                select new
                                {
                                    UserName = user.Name,
                                    ProfilePicture = userDetails.ProfilePicture
                                }).FirstOrDefaultAsync();

            return result == null
                ? (null, null)
                : (result.UserName, result.ProfilePicture);
        }

        public async Task<string> GetUserPicturebyId(int userId)
        {
            return await _dbContext.UserDetails.Where(x => x.UserId == userId).Select(x => x.ProfilePicture).FirstOrDefaultAsync();
        }

        public async Task<User> IsUserExist(int userid)
        {
            return await _dbContext.Users.Where(x => x.Id == userid).FirstOrDefaultAsync();
        }

        //ActiveUser Repository Related 
        public async Task<int> AddActiveUser(ActiveUser activeUser)
        {
            _dbContext.ActiveUsers.Add(activeUser);
            return await Save();
        }
        public async Task<int> UpdateActiveUser(ActiveUser activeUser)
        {
            var userDetail = await GetActiveUserbyId(activeUser.UserId);
            userDetail.LastOnline = DateTime.Now;
            _dbContext.ActiveUsers.Update(userDetail);
            return await Save();
        }

        public Task<List<ActiveUser>> GetAllActiveUser()
        {
            return _dbContext.ActiveUsers.ToListAsync();

        }
        public async Task<ActiveUser> GetActiveUserbyId(int userId)
        {
            return await _dbContext.ActiveUsers.FirstOrDefaultAsync(x => x.UserId == userId);
        }
        public async Task<int> isExist(int userid)
        {
            return await _dbContext.ActiveUsers.Where(x => x.UserId == userid).Select(usr => usr.Id).FirstOrDefaultAsync();

        }
        public async Task<string> GetConnectionIdByuserId(int userid)
        {
            return await _dbContext.ActiveUsers.Where(x => x.UserId == userid).Select(usr => usr.ConnectionId).FirstOrDefaultAsync();
        }

        public async Task<int> RemoveActiveUser(int id)
        {
            var result = await _dbContext.ActiveUsers.FindAsync(id);

            if (result != null)
            {
                _dbContext.ActiveUsers.RemoveRange(result);
                return await Save();
            }
            return 0;

        }
        private async Task<int> Save()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}