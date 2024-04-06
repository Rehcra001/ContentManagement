﻿using ContentManagement.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentManagement.WPF.Validators
{
    public class NewUserValidator : AbstractValidator<UserRegistrationDTO>
    {
        CustomValidationRules _vr = new CustomValidationRules();

        public NewUserValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            //Email Address
            RuleFor(u => u.EmailAddress)
                .NotEmpty().WithMessage("{PropertyName} may not be empty.")
                .EmailAddress().WithMessage("{PropertyName} is not valid.")
                .MaximumLength(100).WithMessage("{PropertyName} cannot be more than {MaxLength} characters.")
                .Must(_vr.BeAValidEmailCharacter).WithMessage("{PropertyName} contains invalid characters! ';' or '--'");

            //Confirm Email Address
            RuleFor(u => u.ConfirmEmailAddress).Equal(u => u.EmailAddress).WithMessage("{PropertyName} does not match Email Address.");

            //Password
            RuleFor(u => u.Password)
                .NotEmpty().WithMessage("{PropertyName} may not be empty.")
                .MinimumLength(8).WithMessage("{PropertyName} must contain at least {MinLength} characters.")
                .MaximumLength(80).WithMessage("{PropertyName} cannot be more than {MaxLength} charaters.");

            //Confirm Password
            RuleFor(u => u.ConfirmPassword).Equal(u => u.Password).WithMessage("{PropertyName} does not match Password.");

            //First Name
            RuleFor(u => u.FirstName)
                .NotEmpty().WithMessage("{PropertyName} may not be empty.")
                .Length(2, 100).WithMessage("{PropertyName} must be between {MinLength} and {MaxLength}.")
                .Must(_vr.BeAValidName).WithMessage("{PropertyName} may only contain letters, space or dash characters.");

            //Last Name
            RuleFor(u => u.LastName)
                .NotEmpty().WithMessage("{PropertyName} may not be empty.")
                .Length(2, 100).WithMessage("{PropertyName} must be between {MinLength} and {MaxLength}.")
                .Must(_vr.BeAValidName).WithMessage("{PropertyName} may only contain letters, space or dash characters.");

            //Display Name
            RuleFor(u => u.DisplayName)
                .NotEmpty().WithMessage("{PropertyName} may not be empty.")
                .Length(3, 50).WithMessage("{PropertyName} must be between {MinLength} and {MaxLength}.")
                .Must(_vr.BeAValidName).WithMessage("{PropertyName} may only contain letters, space or dash characters.");

            //Role
            RuleFor(u => u.Role)
                .NotEmpty().WithMessage("{PropertyName} may not be empty.")
                .MaximumLength(80).WithMessage("{PropertyName} cannot be more than {MaxLength} charaters.")
                .Must(_vr.BeAValidRole).WithMessage("{PropertyName} is not a valid role.");
        }
    }
}
