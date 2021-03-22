using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PlayTogether.Client.ViewModels
{
    public class ChangePasswordViewModel
    {
        public string OldPassword { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordValidator : AbstractValidator<ChangePasswordViewModel>
    {
        public ChangePasswordValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(x => x.OldPassword)
                .NotEmpty()
                .WithMessage("Please enter your current password.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("Please enter new password.")
                .MinimumLength(6).WithMessage("The new password must be at least 6 characters long.")
                .MaximumLength(100).WithMessage("The new password must be a maximum of 100 characters long.")
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
                .NotEmpty().WithMessage("Please re-enter new password.")
                .Equal(x => x.NewPassword).WithMessage("The new password and confirmation password do not match.");
        }
    }
}
