using ContentManagement.DTOs;
using FluentValidation;

namespace ContentManagement.WPF.Validators
{
    public class ChangePasswordValidator : AbstractValidator<ChangePasswordDTO>
    {
        CustomValidationRules _vr = new CustomValidationRules();
        public ChangePasswordValidator()
        {
            //Old Password
            RuleFor(u => u.OldPassword)
                .NotEmpty().WithMessage("{PropertyName} may not be empty.")
                .MinimumLength(8).WithMessage("{PropertyName} must contain at least {MinLength} characters.")
                .MaximumLength(80).WithMessage("{PropertyName} cannot be more than {MaxLength} charaters.");

            //New Password
            RuleFor(u => u.NewPassword)
                .NotEmpty().WithMessage("{PropertyName} may not be empty.")
                .MinimumLength(8).WithMessage("{PropertyName} must contain at least {MinLength} characters.")
                .MaximumLength(80).WithMessage("{PropertyName} cannot be more than {MaxLength} charaters.");

            //Confirm Password
            RuleFor(u => u.ConfirmPassword).Equal(u => u.ConfirmPassword).WithMessage("{PropertyName} does not match Password.");

        }
    }
}
