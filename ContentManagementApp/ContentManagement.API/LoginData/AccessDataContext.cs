using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ContentManagement.API.LoginData
{
    public class AccessDataContext : IdentityDbContext<ApplicationUser>
    {
        public AccessDataContext(DbContextOptions<AccessDataContext> options): base(options)
        {
            
        }
    }
}
