using ContentManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ContentManagement.API.Helpers;

namespace ContentManagement.API.LoginData
{
    public class UserRepository : IUserRepository
    {
        private readonly AccessDataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(AccessDataContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<UserModel> GetUser(string email)
        {
            ApplicationUser? user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new UserModel();
            }
            else
            {
                UserModel userModel = new UserModel
                {
                    EmailAddress = user.Email,
                    DisplayName = user.DisplayName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = await MainUserRole.GetUserMainRole(_userManager, user)
                };

                return userModel;
            }

        }

        public async Task<IEnumerable<UserModel>> GetUsers()
        {
            var users = await (from user in _context.Users
                               join userRoles in _context.UserRoles on user.Id equals userRoles.UserId
                               join role in _context.Roles on userRoles.RoleId equals role.Id
                               where role.Name == "Administrator" || role.Name == "Author"
                               select new UserModel
                               {
                                   EmailAddress = user.Email,
                                   DisplayName = user.DisplayName,
                                   FirstName = user.FirstName,
                                   LastName = user.LastName,
                                   Role = role.Name
                               }).ToListAsync();
            return users;
        }

        public Task<bool> RemoveUser()
        {
            throw new NotImplementedException();
        }

        public Task<UserModel> UpdateUser(string email)
        {
            throw new NotImplementedException();
        }

    }
}
