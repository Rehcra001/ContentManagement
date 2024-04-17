using System.ComponentModel.DataAnnotations;

namespace ContentManagement.Models
{
    public class PostSubCategoryModel
    {
        public int Id { get; set; }
        [Required]
        public int PostId { get; set; }
        [Required]
        public int PostSubCategoryId { get; set; }
        public SubCategoryModel PostSubCategory { get; set; } = new SubCategoryModel();
    }
}
