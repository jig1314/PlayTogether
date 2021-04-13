using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
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
    public class DeleteAccountBase : ComponentBase
    {
        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        public AuthenticationState AuthenticationState { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IUserService UserService { get; set; }

        public string ErrorMessage { get; set; }

        public bool SubmittingData { get; set; } = false;

        public DeleteAccountViewModel DeleteAccountViewModel { get; set; }

        protected override async Task OnInitializedAsync()
        {
            AuthenticationState = await AuthenticationStateTask;

            if (!AuthenticationState.User.Identity.IsAuthenticated)
            {
                NavigationManager.NavigateTo($"/login/{Uri.EscapeDataString(NavigationManager.Uri)}");
            }
            else
            {
                DeleteAccountViewModel = new DeleteAccountViewModel();
            }
        }

        protected async Task DeleteAccount()
        {
            var deleteAccountDto = new DeleteAccountDto()
            {
                Password = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(DeleteAccountViewModel.Password))
            };

            ErrorMessage = null;

            try
            {
                SubmittingData = true;

                await UserService.DeleteAccount(deleteAccountDto);
                NavigationManager.NavigateTo("/", true);
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
