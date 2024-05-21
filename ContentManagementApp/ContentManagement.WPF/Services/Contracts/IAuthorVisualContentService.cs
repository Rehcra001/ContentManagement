using ContentManagement.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentManagement.WPF.Services.Contracts
{
    public interface IAuthorVisualContentService
    {
        Task<AuthorVisualContentDTO?> AddAuthorVisualContent(AuthorVisualContentDTO? authorVisualContent, string localFilePath);
        Task<bool> DeleteAuthorVisualContent(int id);
        Task<IEnumerable<AuthorVisualContentDTO>> GetAllAuthorVisualContent();
        Task<AuthorVisualContentDTO> GetAuthorVisualContent(int id);
        Task<bool> UpdateAuthorVisualContent(AuthorVisualContentDTO authorVisualContent, string localFilePath);
    }
}
