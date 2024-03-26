using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentManagement.Models.ValidationClasses
{
    public class ValidationMessage
    {
        public string? PropertyName { get; set; }
        public string? ErrorMessage { get; set; }

        public override string ToString()
        {
            if (String.IsNullOrEmpty(PropertyName))
            {
                return $"{ErrorMessage}";
            }
            else
            {
                return $"{ErrorMessage} ({PropertyName})";
            }
        }
    }
}
