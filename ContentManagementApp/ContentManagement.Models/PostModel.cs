using System.ComponentModel.DataAnnotations;
using System.Security.AccessControl;

namespace ContentManagement.Models
{
    public class PostModel
    {
        public int Id { get; set; }
        [Required]
        public int AuthorId { get; set; }
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = "";
        [StringLength(250)]
        public string Summary { get; set; } = "";
        public string PostContent { get; set; } = "";
        public int CategoryId { get; set; }
        public bool IsPublished { get; set; }
        [Required]
        public DateTime CreatedOn { get; set; }
        public DateTime LastModified { get; set; }
        public DateTime PublishedOn { get; set; }
        public CategoryModel Category { get; set; } = new CategoryModel();
        public IEnumerable<PostSubCategoryModel> SubCategories { get; set; } = new List<PostSubCategoryModel>();
        public IEnumerable<PostVisualContentModel> VisualContents { get; set; } = new List<PostVisualContentModel>();
        public IEnumerable<PostTagModel> PostTags { get; set; } = new List<PostTagModel>();
        public IEnumerable<CommentModel> Comments { get; set; } = new List<CommentModel>();
    }
}
