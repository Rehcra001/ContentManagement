using ContentManagement.DTOs;

namespace ContentManagement.WPF.Services.Contracts
{
    public interface IUserService
    {
        Task<bool> RegisterNewUser(UserRegistrationDTO userRegistrationDTO);
        Task<bool> LoginUser(UserSignInDTO userSignInDTO);
        Task ChangeUserPassword(ChangePasswordDTO changePasswordDTO);

        event Action<bool> OnLoggedInChanged;
        void RaiseEventOnLoggedInChanged(bool isLoggedIn);
    }
}
