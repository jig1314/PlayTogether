using FluentValidation;
using FluentValidation.Validators;
using PlayTogether.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PlayTogether.Client.ViewModels
{
    public class RegisterViewModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public int? CountryOfResidenceId { get; set; }

        public int? GenderId { get; set; }
    }

    public class RegisterValidator : AbstractValidator<RegisterViewModel>
    {
        public RegisterValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("Please enter a valid first name.")
                .Matches("^([A-Z][a-zA-Z]{1,})$")
                .WithMessage("Please enter a valid first name.");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Please enter a valid last name.")
                .Matches("^([A-Z][a-zA-Z]{1,})$")
                .WithMessage("Please enter a valid last name.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Please enter valid email address")
                .EmailAddress(EmailValidationMode.AspNetCoreCompatible)
                .WithMessage("Please enter valid email address");

            RuleFor(x => x.UserName)
                .NotEmpty()
                .WithMessage("Please enter a valid username.")
                .Matches("^(?=[a-zA-Z0-9._]{6,20}$)(?!.*[_.]{2})[^_.].*[^_.]$")
                .WithMessage("Please enter a valid username.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Please enter password.")
                .MinimumLength(6).WithMessage("The password must be at least 6 characters long.")
                .MaximumLength(100).WithMessage("The password must be a maximum of 100 characters long.")
                .Custom((password, context) =>
                {
                    var hasNumber = new Regex(@"[0-9]+");
                    var hasLowerChar = new Regex(@"[a-z]+");
                    var hasUpperChar = new Regex(@"[A-Z]+");
                    var hasMinimum6Chars = new Regex(@".{6,}");
                    var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

                    var isValid = hasNumber.IsMatch(password) && hasLowerChar.IsMatch(password) && hasUpperChar.IsMatch(password) && hasMinimum6Chars.IsMatch(password) && hasSymbols.IsMatch(password);
                    if (!isValid)
                    {
                        context.AddFailure("Please enter a valid password.");
                    }
                });

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Please re-enter password.")
                .Equal(x => x.Password).WithMessage("The password and confirmation password do not match.");

            RuleFor(x => x.DateOfBirth).Must(ValidDate)
                .WithMessage("Please enter your Date of Birth.");

            RuleFor(x => x.CountryOfResidenceId)
                .NotEmpty()
                .WithMessage("Please select your Country of Residence.")
                .GreaterThan(0)
                .WithMessage("Please select your Country of Residence.");

            RuleFor(x => x.GenderId)
                .NotEmpty()
                .WithMessage("Please select your Gender.")
                .GreaterThan(0)
                .WithMessage("Please select your Gender.");
        }

        private bool ValidDate(DateTime? date)
        {
            if (date.HasValue)
            {
                return DateTime.Now.Year - date.Value.Year < 150 && DateTime.Now.Date > date.Value.Date;
            }

            return false;
        }
    }
}
