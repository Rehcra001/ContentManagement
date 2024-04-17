using ContentManagement.Models;

namespace ContentManagement.Repositories.Contracts
{
    public interface ISubCategoryRepository
    {
        Task<SubCategoryModel> AddSubCategory(SubCategoryModel subCategory);
        Task<bool> CanDeleteSubCategory(int id);
        Task<bool> DeleteSubCategory(int id);
        Task<IEnumerable<SubCategoryModel>> GetSubCategories();
        Task<SubCategoryModel> GetSubCategory(int id);
        Task<bool> UpdateSubCategory(SubCategoryModel subCategory);
    }
}
