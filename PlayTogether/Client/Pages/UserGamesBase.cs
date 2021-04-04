using BlazorStrap;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using PlayTogether.Client.Services;
using PlayTogether.Shared.DTOs;
using PlayTogether.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlayTogether.Client.Pages
{
    public class UserGamesBase : ComponentBase
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

        public List<UserGameDto> Games { get; set; }

        public List<GameSkillLevel> GameSkillLevels { get; set; }

        public BSModal GameSearchModal { get; set; }

        public GameSearch GameSearchPageForModal { get; set; }

        public bool SubmittingData { get; set; } = false;

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

        protected async Task OnHideModal()
        {
            Games = null;
            GameSkillLevels = null;

            SubmittingData = true;
            await Task.Delay(2000);
            await RefreshData();
        }

        private async Task RefreshData()
        {
            SubmittingData = true;
            GameSkillLevels = await GameService.GetGameSkillLevels();
            Games = await UserService.GetUserGames();
            SubmittingData = false;
        }

        protected async Task RefreshDataModal()
        {
            if (GameSearchPageForModal != null)
                await GameSearchPageForModal.RefreshUserGames();
        }

        protected async Task Unfavorited(UserGameDto game)
        {
            Games.Remove(game);
            await UserService.RemoveUserGame(game.ApiId);
        }

        protected async Task OnSkillLevelChanged(UserGameDto game)
        {
            await UserService.UpdateUserGameSkillLevel(new UserGameDto()
            {
                Id = game.Id,
                ApiId = game.ApiId,
                Name = game.Name,
                Summary = game.Summary,
                ReleaseDate = game.ReleaseDate,
                ImageUrl = game.ImageUrl,
                GameSkillLevelId = (game.GameSkillLevelId == 0) ? null : game.GameSkillLevelId
            });
        }
    }
}
