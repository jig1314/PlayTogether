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
    public class LoginBase : ComponentBase
    {
        [Parameter]
        public string ReturnUrl { get; set; }

        [Inject]
        public IUnauthorizedUserService UserService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        public LoginViewModel LoginViewModel { get; set; }

        public string ErrorMessage { get; set; }

        protected override void OnInitialized()
        {
            LoginViewModel = new LoginViewModel();
        }

        protected void GoToRegisterPage()
        {
            NavigationManager.NavigateTo("register");
        }

        protected void GoToForgetYourPasswordPage()
        {
            NavigationManager.NavigateTo("resetPassword");
        }

        protected async Task Login()
        {
            var loginDto = new LoginDto()
            {
                UserName = LoginViewModel.UserName,
                Password = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(LoginViewModel.Password)),
                RememberMe = LoginViewModel.RememberMe
            };

            ErrorMessage = null;

            try
            {
                await UserService.Login(loginDto);
                NavigationManager.NavigateTo("authentication/login");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"{ex.Message}";
            }
        }
    }
}
