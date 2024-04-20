using System.ComponentModel.DataAnnotations;

namespace ContentManagement.Models
{
    public class PostVisualContentModel
    {
        public int Id { get; set; }
        [Required]
        public int PostId { get; set; }
        [Required]
        public int PostVisualContentId { get; set; }
        public AuthorVisualContentModel PostVisualContent { get; set; } = new AuthorVisualContentModel();
    }
}
