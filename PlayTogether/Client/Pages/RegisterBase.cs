using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlayTogether.Client.ViewModels;
using PlayTogether.Shared.Models;
using PlayTogether.Client.Services;
using PlayTogether.Shared.DTOs;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace PlayTogether.Client.Pages
{
    public class RegisterBase : ComponentBase
    {
        [Parameter]
        public string ReturnUrl { get; set; }

        [Inject]
        public IUnauthorizedUserService UserService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        public List<Gender> Genders { get; set; }

        public List<Country> Countries { get; set; }

        public RegisterViewModel RegisterViewModel { get; set; }

        public string ErrorMessage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            RegisterViewModel = new RegisterViewModel();
            Genders = await UserService.GetGenders();
            Countries = await UserService.GetCountries();
        }

        protected async Task RegisterUser()
        {
            var registerUserDto = new RegisterUserDto()
            {
                FirstName = RegisterViewModel.FirstName,
                LastName = RegisterViewModel.LastName,
                Email = RegisterViewModel.Email,
                UserName = RegisterViewModel.UserName,
                Password = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(RegisterViewModel.Password)),
                GenderId = RegisterViewModel.GenderId.Value,
                CountryOfResidenceId = RegisterViewModel.CountryOfResidenceId.Value,
                DateOfBirth = RegisterViewModel.DateOfBirth.Value
            };

            ErrorMessage = null;

            try
            {
                await UserService.RegisterNewUser(registerUserDto);
                NavigationManager.NavigateTo("authentication/login");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"{ex.Message}";
            }
        }
    }
}
