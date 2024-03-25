using System.ComponentModel.DataAnnotations;

namespace ContentManagement.BlazorServer.Data.UserValidators
{
    public class User_UniqueEmailAddressAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || String.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult("Email address is required");
            }

            var context = (ApplicationDbContext)validationContext.GetRequiredService(typeof(ApplicationDbContext));

            var user = context.Users.FirstOrDefault(x => x.Email == value.ToString());

            if (user != null)
            {
                return new ValidationResult("Email address already exists. Please use the login in page with your password");
            }

            return ValidationResult.Success;
        }
    }
}
