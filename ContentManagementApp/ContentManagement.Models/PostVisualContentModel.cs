using System.ComponentModel.DataAnnotations;

namespace ContentManagement.Models
{
    public class PostVisualContentModel
    {
        public int Id { get; set; }
        [Required]
        public int AuthorId { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = "";
        [Required]
        [StringLength(250)]
        public string Description { get; set; } = "";
        [Required]
        [StringLength(250)]
        public string ServerLocation { get; set; } = "";
        [Required]
        [StringLength(250)]
        public string LocalLocation { get; set; } = "";
    }
}
