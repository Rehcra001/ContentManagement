using ContentManagement.Models;

namespace ContentManagement.BlazorServer.Services.Contracts
{
    public interface IPersonService
    {
        Task<PersonModel> AddPerson(PersonModel person);
        Task<bool> PersonExists(string username);
        Task<bool> UpdatePerson(PersonModel person);
    }
}
