using ContentManagement.DTOs;
using ContentManagement.Models;

namespace ContentManagement.API.Extensions
{
    public static class ConvertToModel
    {
        public static UserRegistrationModel ConvertToUserRegistrationModel(this UserRegistrationDTO userRegistrationDTO)
        {
            return new UserRegistrationModel
            {
                EmailAddress = userRegistrationDTO.EmailAddress,
                ConfirmEmailAddress = userRegistrationDTO.ConfirmEmailAddress,
                Password = userRegistrationDTO.Password,
                ConfirmPassword = userRegistrationDTO.ConfirmPassword,
                DisplayName = userRegistrationDTO.DisplayName,
                FirstName = userRegistrationDTO.FirstName,
                LastName = userRegistrationDTO.LastName,
                Role = userRegistrationDTO.Role
            };
        }

        public static UserSignInModel ConvertToUserSignInModel(this UserSignInDTO userSignInDTO)
        {
            return new UserSignInModel
            {
                EmailAddress = userSignInDTO.EmailAddress,
                Password = userSignInDTO.Password
            };
        }
    }
}
