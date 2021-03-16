using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using PlayTogether.Client.Services;
using PlayTogether.Client.ViewModels;
using PlayTogether.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlayTogether.Client.Pages
{
    public class ChangePasswordBase : ComponentBase
    {
        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        public AuthenticationState AuthenticationState { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IUserService UserService { get; set; }

        public ChangePasswordViewModel ChangePasswordViewModel { get; set; }

        public string ErrorMessage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            AuthenticationState = await AuthenticationStateTask;

            if (!AuthenticationState.User.Identity.IsAuthenticated)
            {
                NavigationManager.NavigateTo("/login");
            }
            else
            {
                ChangePasswordViewModel = new ChangePasswordViewModel();
            }
        }

        protected async Task UpdatePassword()
        {
            var changePasswordDto = new ChangePasswordDto()
            {
                OldPassword = ChangePasswordViewModel.OldPassword,
                NewPassword = ChangePasswordViewModel.NewPassword
            };

            ErrorMessage = null;

            try
            {
                await UserService.UpdatePassword(changePasswordDto);
                NavigationManager.NavigateTo("manageProfile/myAccount");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"{ex.Message}";
            }
        }
    }
}
