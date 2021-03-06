using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using PlayTogether.Client.Services;
using PlayTogether.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlayTogether.Client.Pages
{
    public class UserGamingPlatformsBase : ComponentBase
    {
        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        public AuthenticationState AuthenticationState { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IUserService UserService { get; set; }

        [Inject]
        public IGameService GameService { get; set; }

        public List<GamingPlatformDto> GamingPlatforms { get; set; }

        public List<int> UserGamingPlatformIds { get; set; }

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
                SubmittingData = true;
                GamingPlatforms = await GameService.GetGamingPlatforms();
                UserGamingPlatformIds = (await UserService.GetUserGamingPlatforms()).Select(p => p.Id).ToList();
                SubmittingData = false;
            }
        }

        protected async Task Favorited(GamingPlatformDto gamingPlatform)
        {
            UserGamingPlatformIds.Add(gamingPlatform.Id);
            await UserService.AddUserGamingPlatform(gamingPlatform);
        }

        protected async Task Unfavorited(GamingPlatformDto gamingPlatform)
        {
            UserGamingPlatformIds.Remove(gamingPlatform.Id);
            await UserService.RemoveUserGamingPlatform(gamingPlatform);
        }
    }
}
