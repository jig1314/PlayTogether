using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using PlayTogether.Client.Services;
using PlayTogether.Client.ViewModels;
using PlayTogether.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayTogether.Client.Pages
{
    public class ResetPasswordBase : ComponentBase
    {
        [Inject]
        public IUnauthorizedUserService UserService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        public ResetPasswordViewModel ResetPasswordViewModel { get; set; }

        public string ErrorMessage { get; set; }

        public bool SubmittingData { get; set; } = false;

        protected override void OnInitialized()
        {
            ResetPasswordViewModel = new ResetPasswordViewModel();
        }

        protected async Task ResetPassword()
        {
            var resetPasswordDto = new ResetPasswordDto()
            {
                UserName = ResetPasswordViewModel.UserName,
                Password = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(ResetPasswordViewModel.Password))
            };

            ErrorMessage = null;

            try
            {
                SubmittingData = true;
                await UserService.ResetPassword(resetPasswordDto);
                NavigationManager.NavigateTo("login");
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
