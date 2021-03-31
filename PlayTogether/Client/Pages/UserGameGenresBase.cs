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
    public class UserGameGenresBase : ComponentBase
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

        public List<GameGenreDto> GameGenres { get; set; }

        public List<string> UserGameGenreIds { get; set; }

        protected override async Task OnInitializedAsync()
        {
            AuthenticationState = await AuthenticationStateTask;

            if (!AuthenticationState.User.Identity.IsAuthenticated)
            {
                NavigationManager.NavigateTo("/login");
            }
            else
            {
                await RefreshData();
            }
        }

        protected async Task SaveGameGenresAsync()
        {
            GameGenres = null;
            await UserService.UpdateUserGameGenres(UserGameGenreIds.ConvertAll((item) => Convert.ToInt32(item)));
            await RefreshData();
        }

        private async Task RefreshData()
        {
            GameGenres = await GameService.GetGameGenres();
            UserGameGenreIds = (await UserService.GetUserGameGenres()).Select(p => p.Id.ToString()).ToList();
        }
    }
}
