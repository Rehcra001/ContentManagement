using System.ComponentModel.DataAnnotations;

namespace ContentManagement.Models
{
    public class AuthorVisualContentModel
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = "";
        [Required]
        [StringLength(250)]
        public string Description { get; set; } = "";
        [Required]
        [StringLength(2000)]
        public string FileName { get; set; } = "";
        [Required]
        [StringLength(50)]
        public string VisualContentType { get; set; } = "";
        public bool IsHttpLink { get; set; }
    }
}
