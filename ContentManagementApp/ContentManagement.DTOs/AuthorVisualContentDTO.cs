using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentManagement.DTOs
{
    public class AuthorVisualContentDTO
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string FileName { get; set; } = "";
        public string VisualContentType { get; set; } = "";
        public bool IsHttpLink { get; set; }
    }
}
