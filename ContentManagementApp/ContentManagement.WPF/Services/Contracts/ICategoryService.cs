using ContentManagement.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentManagement.WPF.Services.Contracts
{
    public interface ICategoryService
    {
        Task<CategoryDTO?> AddCategory(CategoryDTO category);
        Task<IEnumerable<CategoryDTO>?> GetCategories();
        Task<CategoryDTO?> GetCategory(int id);
        Task<bool> RemoveCategory(int id);
        Task<bool> UpdateCategory(CategoryDTO category);
    }
}
