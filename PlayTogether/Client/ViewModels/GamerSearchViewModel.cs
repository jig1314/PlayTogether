using FluentValidation;
using PlayTogether.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlayTogether.Client.ViewModels
{
    public class GamerSearchViewModel
    {
        public string SearchCriteria { get; set; }
    }

    public class GamerSearchValidator : AbstractValidator<GamerSearchViewModel>
    {
        public GamerSearchValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(x => x.SearchCriteria)
                .NotEmpty()
                .WithMessage("Please enter search criteria.");
        }
    }
}
