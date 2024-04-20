using ContentManagement.DTOs;
using ContentManagement.Models;

namespace ContentManagement.API.Services.Contracts
{
    public interface IAuthorVisualContentService
    {
        Task<AuthorVisualContentDTO?> AddAuthorVisualContent(AuthorVisualContentDTO authorVisualContent, string username);
        Task<IEnumerable<AuthorVisualContentDTO>> GetAllAuthorVisualContent(int authorId);
        Task<AuthorVisualContentDTO> GetAuthorVisualContent(int id);
        Task<bool> UpdateAuthorVisualContent(AuthorVisualContentDTO authorVisualContent);
        Task<bool> DeleteAuthorVisualContent(int id);
    }
}
