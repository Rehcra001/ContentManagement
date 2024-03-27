using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ContentManagement.BlazorServer.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //Set unique constrainta
            builder.Entity<ApplicationUser>()
                .HasAlternateKey(k => k.Email);
            builder.Entity<ApplicationUser>()
                .HasAlternateKey(k => k.UserName);
            builder.Entity<ApplicationUser>()
                .HasAlternateKey(k => k.DisplayName);


        }
    }

   
}
