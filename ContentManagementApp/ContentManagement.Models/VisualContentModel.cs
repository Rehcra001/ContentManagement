using System.ComponentModel.DataAnnotations;

namespace ContentManagement.Models
{
    public class VisualContentModel
    {
        public int Id { get; set; }
        [Required]
        public int PostId { get; set; }
        [Required]
        public int PostVisualContentId { get; set; }
        public PostVisualContentModel PostVisualContent { get; set; } = new PostVisualContentModel();
    }
}
