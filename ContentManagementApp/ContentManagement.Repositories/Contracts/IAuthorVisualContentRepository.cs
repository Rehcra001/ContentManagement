using ContentManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentManagement.Repositories.Contracts
{
    public interface IAuthorVisualContentRepository
    {
        Task<AuthorVisualContentModel?> AddAuthorVisualContent(AuthorVisualContentModel authorVisualContent);
        Task<bool> CanDeleteAuthorVisualContent(int id);
        Task<bool> DeleteAuthorVisualContent(int id);
        Task<IEnumerable<AuthorVisualContentModel>> GetAllAuthorVisualContent(int authorId);
        Task<AuthorVisualContentModel> GetAuthorVisualContent(int id);
        Task<bool> UpdateAuthorVisualContent(AuthorVisualContentModel authorVisualContent);

    }
}
