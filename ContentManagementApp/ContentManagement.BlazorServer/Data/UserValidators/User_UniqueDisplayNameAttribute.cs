using System.ComponentModel.DataAnnotations;

namespace ContentManagement.BlazorServer.Data.UserValidators
{
    public class User_UniqueDisplayNameAttribute : ValidationAttribute
    {

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || String.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult("Display name is required");
            }

            var context = (ApplicationDbContext)validationContext.GetRequiredService(typeof(ApplicationDbContext));

            var user = context.Users.FirstOrDefault(x => x.DisplayName == value.ToString());
            
            if (user != default)
            {
                return new ValidationResult("Display name already exits. Please enter a new name");
            }

            return ValidationResult.Success;

        }
    }
}
