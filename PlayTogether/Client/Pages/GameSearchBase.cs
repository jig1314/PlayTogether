using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using PlayTogether.Client.Services;
using PlayTogether.Client.ViewModels;
using PlayTogether.Shared.DTOs;
using PlayTogether.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlayTogether.Client.Pages
{
    public class GameSearchBase : ComponentBase
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

        public GameSearchViewModel GameSearchViewModel { get; set; }

        public List<GameSearchResult> Games { get; set; }

        public List<long> UserGamesApiIds { get; set; }

        public bool SubmittingData { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            GameSearchViewModel = new GameSearchViewModel();

            AuthenticationState = await AuthenticationStateTask;

            if (!AuthenticationState.User.Identity.IsAuthenticated)
            {
                NavigationManager.NavigateTo($"/login/{Uri.EscapeDataString(NavigationManager.Uri)}");
            }
            else
            {
                await RefreshUserGames();
            }
        }

        public async Task RefreshUserGames()
        {
            UserGamesApiIds = null;

            var userGames = await UserService.GetUserGames();
            UserGamesApiIds = userGames.Select(game => game.ApiId).ToList();

            StateHasChanged();
        }

        protected async void OnSearchCriteriaChanged(ChangeEventArgs eventArgs)
        {
            if (eventArgs.Value != null)
            {
                var gameSearchDto = new GameSearchDto()
                {
                    SearchCriteria = GameSearchViewModel.SearchCriteria
                };

                SubmittingData = true;
                Games = await GameService.SearchForGames(gameSearchDto);
                SubmittingData = false;
                StateHasChanged();
            }
        }

        protected async Task Favorited(GameSearchResult game)
        {
            UserGamesApiIds.Add(game.ApiId);
            await UserService.AddUserGame(new UserGameDto() 
            {
                ApiId = game.ApiId,
                Name = game.Name,
                Summary = game.Summary,
                ReleaseDate = game.ReleaseDate.GetValueOrDefault(),
                ImageUrl = game.ImageUrl
            });
        }

        protected async Task Unfavorited(GameSearchResult game)
        {
            UserGamesApiIds.Remove(game.ApiId);
            await UserService.RemoveUserGame(game.ApiId);
        }
    }
}
