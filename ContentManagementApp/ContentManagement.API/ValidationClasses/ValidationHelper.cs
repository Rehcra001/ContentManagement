using System.ComponentModel.DataAnnotations;

namespace ContentManagement.API.ValidationClasses
{
    public static class ValidationHelper
    {
        public static List<ValidationMessage> Validate<T>(T model)
        {
            List<ValidationMessage> messages = new List<ValidationMessage>();

            ValidationContext context = new ValidationContext(model, serviceProvider: null, items: null);
            List<ValidationResult> results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(model, context, results, true))
            {
                foreach (ValidationResult result in results)
                {
                    string propertyName = string.Empty;
                    if (result.MemberNames.Any())
                    {
                        propertyName = ((string[])result.MemberNames)[0];
                    }

                    ValidationMessage message = new ValidationMessage()
                    {
                        ErrorMessage = result.ErrorMessage,
                        PropertyName = propertyName
                    };

                    messages.Add(message);
                }
            }
            return messages;
        }
    }
}
