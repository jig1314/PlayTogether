using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PlayTogether.Client.ViewModels
{
    public class MyAccountViewModel
    {
        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public int? CountryOfResidenceId { get; set; }

        public int? GenderId { get; set; }

        public string PhoneNumber { get; set; }
    }

    public class MyAccountValidator : AbstractValidator<MyAccountViewModel>
    {
        public MyAccountValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(x => x.UserName)
                .NotEmpty()
                .WithMessage("Please enter a valid username.")
                .Matches("^(?=[a-zA-Z0-9._]{6,20}$)(?!.*[_.]{2})[^_.].*[^_.]$")
                .WithMessage("Please enter a valid username.");

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
                .EmailAddress()
                .WithMessage("Please enter valid email address");

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

            RuleFor(x => x.PhoneNumber).Custom((phoneNumber, context) =>
            {
                if (string.IsNullOrWhiteSpace(phoneNumber))
                    return;

                Regex phoneNumberFormat = new Regex(@"^(\+\d{1,2}\s?)?1?\-?\.?\s?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$");
                if (!phoneNumberFormat.IsMatch(phoneNumber))
                {
                    context.AddFailure("Please enter a valid phone number.");
                }
            });
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
