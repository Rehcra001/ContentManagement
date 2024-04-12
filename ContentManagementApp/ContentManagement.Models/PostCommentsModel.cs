using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentManagement.Models
{
    public class PostCommentsModel
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public int Level { get; set; }
        [Required]
        public int PersonId { get; set; }
        [Required]
        [StringLength(500)]
        public string CommentContent { get; set; } = "";
        public bool IsPublished { get; set; }
        [Required]
        public DateTime CreatedOn { get; set; }
        public DateTime LastModified { get; set; }
        public DateTime PublishedOn { get; set; }
    }
}
