using ContentManagement.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentManagement.WPF.Validators
{
    public class AuthorVisualContentValidator : AbstractValidator<AuthorVisualContentDTO>
    {
        private CustomValidationRules _vr = new CustomValidationRules();
        public AuthorVisualContentValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("{PropertyName} may not be empty.")
                .MaximumLength(100).WithMessage("{PropertyName} may not have more than {MaxLength} characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("{PropertyName} may not be empty.")
                .MaximumLength(250).WithMessage("{PropertyName} may not have more than {MaxLength} characters.");

            RuleFor(x => x.VisualContentType)
                .NotEmpty().WithMessage("{PropertyName} may not be empty.")
                .MaximumLength(50).WithMessage("{PropertyName} may not have more than {MaxLength} characters.");

            When(x => x.IsHttpLink, () =>
            {
                RuleFor(x => x.FileName)
                    .NotEmpty().WithMessage("{PropertyName} must contain a valid URL.")
                    .Must(_vr.BeAValidUrl).WithMessage("{PropertyName} is not a valid url.");

            });
                
        }
    }
}
