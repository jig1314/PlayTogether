using BlazorStrap;
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
    public class UserProfileBase : ComponentBase
    {

        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        public AuthenticationState AuthenticationState { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IUserService UserService { get; set; }

        public UserProfileDto UserProfileDto { get; set; }

        [Parameter]
        public string UserName { get; set; }

        public BSTabGroup TabGroup { get; set; }

        public BSTab TabAbout { get; set; }

        public BSTab TabGamingPlatforms { get; set; }

        public BSTab TabGameGenres { get; set; }

        public BSTab TabGames { get; set; }

        protected override async Task OnInitializedAsync()
        {
            AuthenticationState = await AuthenticationStateTask;

            if (!AuthenticationState.User.Identity.IsAuthenticated)
            {
                NavigationManager.NavigateTo("/login");
            }
            else
            {
                UserProfileDto = await UserService.GetUserProfileInformation(UserName);
            }
        }
    }
}
