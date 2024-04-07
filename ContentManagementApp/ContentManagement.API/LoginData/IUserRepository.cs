using ContentManagement.Models;

namespace ContentManagement.API.LoginData
{
    public interface IUserRepository
    {
        Task<bool> RemoveUser();
        Task<UserModel> GetUser(string email);
        Task<IEnumerable<UserModel>> GetUsers();
        Task<UserModel> UpdateUser(string email);

    }
}
