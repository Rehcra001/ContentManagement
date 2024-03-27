using Microsoft.AspNetCore.Identity;

namespace ContentManagement.BlazorServer.Data
{
    public static class SeedRoles
    {
        internal async static Task Seed(RoleManager<IdentityRole> adminRoleManager,
                                        RoleManager<IdentityRole> authorRoleManager,
                                        RoleManager<IdentityRole> userRoleManager,
                                        UserManager<ApplicationUser> defaultAdminManager)
        {
            await SeedAdministratorRole(adminRoleManager);
            await SeedAuthorRole(authorRoleManager);
            await SeedUserRole(userRoleManager);
            await SeedDefaultAdministrator(defaultAdminManager);
        }
        private async static Task SeedAdministratorRole(RoleManager<IdentityRole> adminRoleManager)
        {
            bool administratorRoleExits = await adminRoleManager.RoleExistsAsync("Administrator");

            if (administratorRoleExits == false)
            {
                var role = new IdentityRole
                {
                    Name = "Administrator"
                };
                await adminRoleManager.CreateAsync(role);
            }
        }

        private async static Task SeedAuthorRole(RoleManager<IdentityRole> authorRoleManager)
        {
            bool authorRolesExists = await authorRoleManager.RoleExistsAsync("Author");

            if (authorRolesExists == false)
            {
                var role = new IdentityRole
                {
                    Name = "Author"
                };
                await authorRoleManager.CreateAsync(role);
            }
        }

        private async static Task SeedUserRole(RoleManager<IdentityRole> userRoleManager)
        {
            bool userRoleExists = await userRoleManager.RoleExistsAsync("User");

            if (userRoleExists == false)
            {
                var role = new IdentityRole
                {
                    Name = "User"
                };
                await userRoleManager.CreateAsync(role);
            }
        }

        private async static Task SeedDefaultAdministrator(UserManager<ApplicationUser> defaultAdminManager)
        {
            bool adminExists = await defaultAdminManager.FindByEmailAsync("admin@blog.com") != null;

            if (adminExists == false)
            {
                var defaultAdministrator = new ApplicationUser
                {
                    UserName = "admin@blog.com",
                    Email = "admin@blog.com",
                    EmailConfirmed = true,
                    FirstName = "System",
                    LastName = "Admin",
                    DisplayName = "Administrator"
                };
                IdentityResult identityResult = await defaultAdminManager.CreateAsync(defaultAdministrator, "Password1!");

                if (identityResult.Succeeded)
                {
                    await defaultAdminManager.AddToRoleAsync(defaultAdministrator, "Administrator");
                }
            }
        }
    }
}
