using Microsoft.AspNetCore.Components;
using PlayTogether.Client.Services;
using PlayTogether.Client.ViewModels;
using PlayTogether.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlayTogether.Client.Pages
{
    public class ResetPasswordBase : ComponentBase
    {
        [Inject]
        public IUserService UserService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        public ResetPasswordViewModel ResetPasswordViewModel { get; set; }

        public string ErrorMessage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            ResetPasswordViewModel = new ResetPasswordViewModel();
        }

        protected async Task ResetPassword()
        {
            var resetPasswordDto = new ResetPasswordDto()
            {
                UserName = ResetPasswordViewModel.UserName,
                Password = ResetPasswordViewModel.Password
            };

            ErrorMessage = null;

            try
            {
                await UserService.ResetPassword(resetPasswordDto);
                NavigationManager.NavigateTo("login");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"{ex.Message}";
            }
        }
    }
}
