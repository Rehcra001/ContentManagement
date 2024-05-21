using ContentManagement.WPF.Enums;

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

        internal bool BeAValidRole(string role)
        {
            var roles = Enum.GetValues(typeof(Roles));

            foreach (var roleName in roles)
            {
                if (role.Equals(roleName.ToString()))
                {
                    return true;
                }
            }
            
            return false;
        }

        internal bool BeAValidDate(DateTime date)
        {
            if (date != default)
            {
                return true;
            }
            return false;
        }

        internal bool BeAValidUrl(string fileName)
        {
            if (fileName.Substring(0, 7).ToLower().Equals("http://") || fileName.Substring(0, 8).ToLower().Equals("https://"))
            {
                return true;
            }
            return false;
        }
    }
}
