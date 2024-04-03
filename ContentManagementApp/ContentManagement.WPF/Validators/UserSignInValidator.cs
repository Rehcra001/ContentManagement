using ContentManagement.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentManagement.WPF.Validators
{
    public class UserSignInValidator : AbstractValidator<UserSignInDTO>
    {
        CustomValidationRules _vr = new CustomValidationRules();

        public UserSignInValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            //Email Address
            RuleFor(u => u.EmailAddress)
                .NotEmpty().WithMessage("{PropertyName} may not be empty.")
                .MaximumLength(100).WithMessage("{PropertyName} may not contain more than {MaxLength} characters.")
                .EmailAddress().WithMessage("{PropertyName} is not valid.")
                .Must(_vr.BeAValidEmailCharacter).WithMessage("{PropertyName} contains invalid characters! ';' or '--'");

            //Password
            RuleFor(u => u.Password)
                .NotEmpty().WithMessage("{PropertyName} may not be empty.")
                .MinimumLength(8).WithMessage("{PropertyName} must contain at least {MinLength} characters.")
                .MaximumLength(80).WithMessage("{PropertyName} may not contain more than {MaxLength} characters.");
        }
    }
}
