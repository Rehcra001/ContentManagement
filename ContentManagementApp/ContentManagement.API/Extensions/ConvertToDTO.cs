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
    }
}
