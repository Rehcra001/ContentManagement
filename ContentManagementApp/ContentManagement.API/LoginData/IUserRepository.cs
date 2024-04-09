using ContentManagement.Models;

namespace ContentManagement.API.LoginData
{
    public interface IUserRepository
    {
        Task<bool> RemoveUser(string email);
        Task<UserModel> GetUser(string email);
        Task<IEnumerable<UserModel>> GetUsers();
        Task<bool> UpdateUser(UserModel user);

    }
}
