using ContentManagement.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentManagement.WPF.Services.Contracts
{
    public interface ISubCategoryService
    {
        Task<SubCategoryDTO?> AddSubCategory(SubCategoryDTO subCategory);
        Task<IEnumerable<SubCategoryDTO>?> GetSubCategories();
        Task<SubCategoryDTO?> GetSubCategory(int id);
        Task<bool> RemoveSubCategory(int id);
        Task<bool> UpdateSubCategory(SubCategoryDTO subCategory);
    }
}
