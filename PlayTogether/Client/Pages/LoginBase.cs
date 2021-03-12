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
    public class LoginBase : ComponentBase
    {
        [Parameter]
        public string ReturnUrl { get; set; }

        [Inject]
        public IUnauthorizedUserService UserService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        public LoginViewModel LoginViewModel { get; set; }

        protected override async Task OnInitializedAsync()
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
                Password = LoginViewModel.Password,
                RememberMe = LoginViewModel.RememberMe
            };

            await UserService.Login(loginDto);
            NavigationManager.NavigateTo("authentication/login");
        }
    }
}
