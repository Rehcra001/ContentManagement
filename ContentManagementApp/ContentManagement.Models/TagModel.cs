using System.ComponentModel.DataAnnotations;

namespace ContentManagement.Models
{
    public class TagModel
    {
        public int Id { get; set; }
        [Required]
        public int AuthorId { get; set; }
        [Required]
        [StringLength(50)]
        public string TagName { get; set; }
        [Required]
        public DateTime CreatedOn { get; set; }
        public DateTime LastModified { get; set; }
    }
}
