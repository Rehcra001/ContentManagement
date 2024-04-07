using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentManagement.DTOs
{
    public class UserDTO
    {        
        public string? EmailAddress { get; set; }

        public string? DisplayName { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Role { get; set; }
    }
}
