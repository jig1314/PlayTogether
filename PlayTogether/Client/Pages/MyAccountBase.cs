using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using PlayTogether.Client.Services;
using PlayTogether.Client.ViewModels;
using PlayTogether.Shared.DTOs;
using PlayTogether.Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlayTogether.Client.Pages
{
    public class MyAccountBase : ComponentBase
    {
        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        public AuthenticationState AuthenticationState { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IUserService UserService { get; set; }

        public MyAccountViewModel MyAccountViewModel { get; set; }

        public List<Gender> Genders { get; set; }

        public List<Country> Countries { get; set; }

        public string ErrorMessage { get; set; }

        public bool SubmittingData { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            AuthenticationState = await AuthenticationStateTask;

            if (!AuthenticationState.User.Identity.IsAuthenticated)
            {
                NavigationManager.NavigateTo($"/login/{Uri.EscapeDataString(NavigationManager.Uri)}");
            }
            else
            {
                await RefreshData();
            }
        }

        private async Task RefreshData()
        {
            Genders = await UserService.GetGenders();
            Countries = await UserService.GetCountries();

            var userAccountDto = await UserService.GetUserAccountInfo();
            MyAccountViewModel = new MyAccountViewModel()
            {
                UserName = userAccountDto.UserName,
                FirstName = userAccountDto.FirstName,
                LastName = userAccountDto.LastName,
                Email = userAccountDto.Email,
                DateOfBirth = userAccountDto.DateOfBirth,
                CountryOfResidenceId = userAccountDto.CountryOfResidenceId,
                GenderId = userAccountDto.GenderId,
                PhoneNumber = userAccountDto.PhoneNumber
            };
        }

        protected async Task UpdateAccount()
        {
            var userAccountDto = new UserAccountDto()
            {
                UserName = MyAccountViewModel.UserName,
                FirstName = MyAccountViewModel.FirstName,
                LastName = MyAccountViewModel.LastName,
                Email = MyAccountViewModel.Email,
                DateOfBirth = MyAccountViewModel.DateOfBirth.Value,
                CountryOfResidenceId = MyAccountViewModel.CountryOfResidenceId.Value,
                GenderId = MyAccountViewModel.GenderId.Value,
                PhoneNumber = MyAccountViewModel.PhoneNumber
            };

            ErrorMessage = null;

            try
            {
                SubmittingData = true;
                await UserService.UpdateUserAccountInfo(userAccountDto);
                await RefreshData();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"{ex.Message}";
            }
            finally
            {
                SubmittingData = false;
            }
        }
    }
}
