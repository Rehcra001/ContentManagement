using ContentManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ContentManagement.API.Helpers;
using Serilog;
using ILogger = Serilog.ILogger;

namespace ContentManagement.API.LoginData
{
    public class UserRepository : IUserRepository
    {
        private readonly AccessDataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;

        public UserRepository(AccessDataContext context,
                              UserManager<ApplicationUser> userManager,
                              ILogger logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
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
                               orderby user.FirstName, user.LastName
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

        public async Task<bool> RemoveUser(string email)
        {
            ApplicationUser? user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                IdentityResult identityResult = await _userManager.DeleteAsync(user);

                if (identityResult.Succeeded)
                {
                    return true;
                }
                else
                {
                    return false;
                }                
            }
            return false;
        }

        public async Task<bool> UpdateUser(UserModel user)
        {
            
            if (!String.IsNullOrWhiteSpace(user.EmailAddress))
            {
                try
                {
                    ApplicationUser? applicationUser = await _userManager.FindByEmailAsync(user.EmailAddress);


                    if (applicationUser != null)
                    {
                        //Get roles
                        var roles = await _userManager.GetRolesAsync(applicationUser);
                        //remove roles
                        await _userManager.RemoveFromRolesAsync(applicationUser, roles);
                        //add role
                        await _userManager.AddToRoleAsync(applicationUser, user.Role!);
                        //update user detail
                        applicationUser.FirstName = user.FirstName;
                        applicationUser.LastName = user.LastName;
                        applicationUser.DisplayName = user.DisplayName;

                        _context.SaveChanges();
                        return true;
                    }

                    return false;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, ex.Message);
                    return false;
                }                
            }
            return false;
        }

    }
}
