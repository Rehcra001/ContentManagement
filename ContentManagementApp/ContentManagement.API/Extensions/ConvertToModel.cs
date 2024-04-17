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

        public static ChangePasswordModel ConvertToChangePasswordModel(this ChangePasswordDTO changePasswordDTO)
        {
            return new ChangePasswordModel
            {
                OldPassword = changePasswordDTO.OldPassword,
                NewPassword = changePasswordDTO.NewPassword,
                ConfirmPassword = changePasswordDTO.ConfirmPassword
            };
        }

        public static UserModel ConvertToUserModel(this UserDTO userDTO)
        {
            return new UserModel
            {
                EmailAddress = userDTO.EmailAddress,
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                DisplayName = userDTO.DisplayName,
                Role = userDTO.Role
            };
        }

        public static CategoryModel ConvertToCategoryModel(this CategoryDTO categoryDTO)
        {
            return new CategoryModel
            {
                Id = categoryDTO.Id,
                Name = categoryDTO.Name,
                Description = categoryDTO.Description,
                IsPublished = categoryDTO.IsPublished,
                CreatedOn = categoryDTO.CreatedOn,
                LastModified = categoryDTO.LastModified,
                PublishedOn = categoryDTO.PublishedOn
            };
        }

        public static SubCategoryModel ConvertToSubCategoryModel(this SubCategoryDTO subCategoryDTO)
        {
            return new SubCategoryModel
            {
                Id = subCategoryDTO.Id,
                Name = subCategoryDTO.Name,
                Description = subCategoryDTO.Description,
                IsPublished = subCategoryDTO.IsPublished,
                CreatedOn = subCategoryDTO.CreatedOn,
                LastModified = subCategoryDTO.LastModified,
                PublishedOn = subCategoryDTO.PublishedOn
            };
        }
    }
}
