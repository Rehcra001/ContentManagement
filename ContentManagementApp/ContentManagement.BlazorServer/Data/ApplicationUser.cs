using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ContentManagement.BlazorServer.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [StringLength(100)]
        public string? FirstName { get; set; }
        [StringLength(100)]
        public string? LastName { get; set; }
    }

}
