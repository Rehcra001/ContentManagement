using ContentManagement.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentManagement.WPF.Validators
{
    public class CategoryValidator : AbstractValidator<CategoryDTO>
    {
        CustomValidationRules _vr = new CustomValidationRules();
        public CategoryValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("{PropertyName} may not be empty")
                .MaximumLength(50).WithMessage("{PropertyName} cannot be more than {MaxLength} characters");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("{PropertyName} may not be empty")
                .MaximumLength(50).WithMessage("{PropertyName} cannot be more than {MaxLength} characters");

            RuleFor(x => x.CreatedOn)
                .Must(_vr.BeAValidDate).WithMessage("{PropertyName} is not valid");

            
        }
    }
}
