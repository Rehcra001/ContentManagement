using ContentManagement.API.LoginData;
using Microsoft.AspNetCore.Identity;

namespace ContentManagement.API.Helpers
{
    public static class MainUserRole
    {
        public static async Task<string> GetUserMainRole(UserManager<ApplicationUser> userManager, ApplicationUser user)
        {
            IList<string> roles = await userManager.GetRolesAsync(user);

            //Three levels of Roles
            //in order of precedence
            if (roles.Contains("Administrator"))
            {
                return "Administrator";
            }
            else if (roles.Contains("Author"))
            {
                return "Author";
            }
            else //(roles.Contains("User"))
            {
                return "User";
            }
        }
    }
}
