using System.ComponentModel.DataAnnotations;

namespace ContentManagement.Models
{
    public class SubCategoryModel
    {
        public int Id { get; set; }
        [Required]
        public int PostId { get; set; }
        [Required]
        public int PostSubCategoryId { get; set; }
        public PostSubCategoryModel PostSubCategory { get; set; } = new PostSubCategoryModel();
    }
}
