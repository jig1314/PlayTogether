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
    public class AddGamerPopUpBase : ComponentBase
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

        public GamerSearchViewModel GamerSearchViewModel { get; set; }

        public ChatGroupConversation Conversation { get; set; }

        public List<UserBasicInfo> Gamers { get; set; }

        public List<GamingPlatformDto> GamingPlatforms { get; set; }

        public List<string> FilterGamingPlatformIds { get; set; } = new List<string>();

        public List<GameGenreDto> GameGenres { get; set; }

        public List<string> FilterGameGenreIds { get; set; } = new List<string>();

        public List<UserGameDto> Games { get; set; }

        public List<string> FilterGameIds { get; set; } = new List<string>();

        public List<string> GamersInGroup { get; set; } = new List<string>();

        public string IdUser { get; set; }

        public bool SubmittingData { get; set; } = false;

        public bool IsFilterOpen { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            GamerSearchViewModel = new GamerSearchViewModel();

            AuthenticationState = await AuthenticationStateTask;

            if (!AuthenticationState.User.Identity.IsAuthenticated)
            {
                NavigationManager.NavigateTo($"/login/{Uri.EscapeDataString(NavigationManager.Uri)}");
            }
            else
            {
                IdUser = AuthenticationState.User.FindFirst("sub").Value;

                GamingPlatforms = await GameService.GetGamingPlatforms();
                GameGenres = await GameService.GetGameGenres();
                Games = await UserService.GetUserGames();
            }
        }

        protected async void OnSearchCriteriaChanged(ChangeEventArgs eventArgs)
        {
            if (eventArgs.Value != null)
            {
                await SearchForGamers();
            }
        }

        protected async Task ApplyFilter()
        {
            await SearchForGamers();
        }

        protected void ResetFilter()
        {
            FilterGameGenreIds = new List<string>();
            FilterGameIds = new List<string>();
            FilterGamingPlatformIds = new List<string>();
            StateHasChanged();
        }

        private async Task SearchForGamers()
        {
            IsFilterOpen = false;
            StateHasChanged();
            var gamerSearchDto = new GamerSearchDto()
            {
                SearchCriteria = GamerSearchViewModel.SearchCriteria,
                GamingPlatformIds = FilterGamingPlatformIds.ConvertAll((item) => Convert.ToInt32(item)),
                GameGenreIds = FilterGameGenreIds.ConvertAll((item) => Convert.ToInt32(item)),
                GameIds = FilterGameIds.ConvertAll((item) => Convert.ToInt32(item))
            };

            SubmittingData = true;
            Gamers = await UserService.SearchForGamers(gamerSearchDto);
            SubmittingData = false;
            StateHasChanged();
        }

        public void ResetPopUp(ChatGroupConversation conversation)
        {
            Conversation = conversation;
            Gamers = null;
            GamerSearchViewModel.SearchCriteria = null;
            GamersInGroup = conversation.WithUsers.Select(u => u.UserId).ToList();
            ResetFilter();
        }

        public void CheckBoxValueChanged(bool value, string idUser)
        {
            if (value && !GamersInGroup.Contains(idUser))
                GamersInGroup.Add(idUser);
            else if (!value && GamersInGroup.Contains(idUser))
                GamersInGroup.Remove(idUser);

            StateHasChanged();
        }
    }
}
