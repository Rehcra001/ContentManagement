using ContentManagement.Models;
using ContentManagement.Repositories.Contracts;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ContentManagement.BlazorServer.Data.UserValidators
{
    public class User_UniqueDisplayNameAttribute : ValidationAttribute
    {
        private string _emailProperty;

        public User_UniqueDisplayNameAttribute(string emailProperty)
        {
            _emailProperty = emailProperty;
        }

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

            //get all people from ContentManagementDB
            IPersonRepository personRepository = (IPersonRepository)validationContext.GetRequiredService(typeof(IPersonRepository));
            var people = personRepository.GetPeople("");

            //Get email property
            PropertyInfo propertyInfo = validationContext.ObjectType.GetProperty(_emailProperty);
            string email = propertyInfo.GetValue(validationContext.ObjectInstance).ToString();

            //Search for display name
            PersonModel? person = people.FirstOrDefault(x => x.DisplayName.ToLower() == value.ToString().ToLower());
            if (person != null)
            {
                //Check if email address the same as a user may delete their account
                //but there username and display name will remain in ContentManagementDB
                if (person.UserName.ToLower() != email.ToLower())
                {
                    return new ValidationResult("Display name already exits. Please enter a new name");
                }
            }

            return ValidationResult.Success;

        }
    }
}
