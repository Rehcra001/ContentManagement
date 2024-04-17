using ContentManagement.DTOs;
using ContentManagement.Models;
using Microsoft.AspNetCore.Identity;

namespace ContentManagement.API.Extensions
{
    public static class ConvertToDTO
    {
        public static IEnumerable<UserDTO> ConvertToUserDTOs(this IEnumerable<UserModel> userModels)
        {
            var results = from user in userModels
                          select new UserDTO
                          {
                              EmailAddress = user.EmailAddress,
                              DisplayName = user.DisplayName,
                              FirstName = user.FirstName,
                              LastName = user.LastName,
                              Role = user.Role
                          };

            return results;
        }

        public static UserDTO ConvertToUserDTO(this UserModel userModel)
        {
            UserDTO userDTO = new UserDTO
            {
                EmailAddress = userModel.EmailAddress,
                DisplayName = userModel.DisplayName,
                FirstName = userModel.FirstName,
                LastName = userModel.LastName,
                Role = userModel.Role
            };

            return userDTO;
        }

        public static IEnumerable<CategoryDTO> ConvertToCategoryDTOs(this IEnumerable<CategoryModel> categoryModels)
        {
            return (from category in categoryModels
                    select new CategoryDTO
                    {
                        Id = category.Id,
                        Name = category.Name,
                        Description = category.Description,
                        IsPublished = category.IsPublished,
                        CreatedOn = category.CreatedOn,
                        LastModified = category.LastModified,
                        PublishedOn = category.PublishedOn
                    });
        }

        public static CategoryDTO ConvertToCategoryDTO(this CategoryModel categoryModel)
        {
            return new CategoryDTO
            {
                Id = categoryModel.Id,
                Name = categoryModel.Name,
                Description = categoryModel.Description,
                IsPublished = categoryModel.IsPublished,
                CreatedOn = categoryModel.CreatedOn,
                LastModified = categoryModel.LastModified,
                PublishedOn = categoryModel.PublishedOn
            };
        }

        public static IEnumerable<SubCategoryDTO> ConvertToSubCategoryDTOs(this IEnumerable<SubCategoryModel> subCategoryModels)
        {
            return (from subCategory in subCategoryModels
                    select new SubCategoryDTO
                    {
                        Id = subCategory.Id,
                        Name = subCategory.Name,
                        Description = subCategory.Description,
                        IsPublished = subCategory.IsPublished,
                        CreatedOn = subCategory.CreatedOn,
                        LastModified = subCategory.LastModified,
                        PublishedOn = subCategory.PublishedOn
                    });
        }

        public static SubCategoryDTO ConvertToSubCategoryDTO(this SubCategoryModel subCategoryModel)
        {
            return new SubCategoryDTO
            {
                Id = subCategoryModel.Id,
                Name = subCategoryModel.Name,
                Description = subCategoryModel.Description,
                IsPublished = subCategoryModel.IsPublished,
                CreatedOn = subCategoryModel.CreatedOn,
                LastModified = subCategoryModel.LastModified,
                PublishedOn = subCategoryModel.PublishedOn
            };
        }
    }
}
