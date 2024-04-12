using System.ComponentModel.DataAnnotations;

namespace ContentManagement.Models
{
    public class CategoryModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = "";
        [Required]
        [StringLength(250)]
        public string Description { get; set; } = "";
        public bool IsPublished { get; set; }
        [Required]
        public DateTime CreatedOn { get; set; }
        public DateTime LastModified { get; set; }
        public DateTime PublishedOn { get; set; }
    }
}
