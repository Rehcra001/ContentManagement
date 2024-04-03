using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentManagement.WPF.Validators
{
    internal class CustomValidationRules
    {
        internal bool BeAValidEmailCharacter(string email)
        {
            if (email.Contains(';'))
            {
                return false;
            }
            if (email.Contains("--"))
            {
                return false;
            }
            return true;
        }

        internal bool BeAValidName(string name)
        {
            name = name.Replace(" ", "");
            name = name.Replace("-", "");

            return name.All(char.IsLetter);
        }
    }
}
