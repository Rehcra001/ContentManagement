using ContentManagement.DTOs;

namespace ContentManagement.WPF.Services.Contracts
{
    public interface IUserService
    {
        Task<bool> RegisterNewUser(UserRegistrationDTO userRegistrationDTO);
        Task<bool> LoginUser(UserSignInDTO userSignInDTO);
        Task ChangeUserPassword(ChangePasswordDTO changePasswordDTO);
        Task<UserDTO> GetUser();
        Task<UserDTO> GetUser(string email);
        Task<IEnumerable<UserDTO>> GetUsers();
        Task<bool> RemoveUser(string email);
        Task<bool> UpdateUser(UserDTO userDTO);

        event Action<bool> OnLoggedInChanged;
        void RaiseEventOnLoggedInChanged(bool isLoggedIn);
    }
}
