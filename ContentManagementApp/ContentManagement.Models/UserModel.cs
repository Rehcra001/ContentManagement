using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentManagement.Models
{
    public class UserModel
    {
        [Required]
        [EmailAddress]
        [StringLength(100)]
        [Display(Name = "Email Address")]
        public string? EmailAddress { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Display Name")]
        public string? DisplayName { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3)]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Role")]
        public string? Role { get; set; }
    }
}
