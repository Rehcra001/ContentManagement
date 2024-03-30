using System.ComponentModel.DataAnnotations;

namespace ContentManagement.Models
{
    public class UserRegistrationModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string? EmailAddress { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Confirm Email Address")]
        [Compare(nameof(EmailAddress), ErrorMessage = "Emails are not the same. Please re-enter your email address.")]
        public string? ConfirmEmailAddress { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(80,  MinimumLength = 8)]
        [Display(Name = "Password")]
        public string? Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords are not the same.")]
        [Display(Name = "Confirm Password")]
        public string? ConfirmPassword { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Display Name")]
        public string? DisplayName { get; set; }

        [Required]
        [StringLength(100)]
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
