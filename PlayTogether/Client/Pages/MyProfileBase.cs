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
    public class MyProfileBase : ComponentBase
    {

        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        public AuthenticationState AuthenticationState { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IUserService UserService { get; set; }

        public UserProfileDto UserProfileDto { get; set; }

        public BSTabGroup TabGroup { get; set; }

        public BSTab TabAbout { get; set; }

        public BSTab TabGamingPlatforms { get; set; }

        public BSTab TabGameGenres { get; set; }

        public BSTab TabGames { get; set; }

        public List<FriendRequestDto> ActiveReceivedFriendRequests { get; set; }

        public BSModal FriendModal { get; set; }

        public FriendPopUp FriendPopUp { get; set; }

        public bool RetrievingData { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            AuthenticationState = await AuthenticationStateTask;

            if (!AuthenticationState.User.Identity.IsAuthenticated)
            {
                NavigationManager.NavigateTo($"/login/{Uri.EscapeDataString(NavigationManager.Uri)}");
            }
            else
            {
                await RefreshData();
            }
        }

        private async Task RefreshData()
        {
            RetrievingData = true;

            var userName = AuthenticationState.User.Identity.Name;
            var idUser = AuthenticationState.User.FindFirst("sub").Value;

            UserProfileDto = await UserService.GetUserProfileInformation(userName);

            var activeFriendRequests = await UserService.GetActiveFriendRequests();
            ActiveReceivedFriendRequests = activeFriendRequests.Where(request => request.ToUserId == idUser).ToList();

            RetrievingData = false;
        }
        
        protected async Task OnHideModal()
        {
            UserProfileDto = null;
            ActiveReceivedFriendRequests = null;

            RetrievingData = true;
            await Task.Delay(2000);
            await RefreshData();
        }

        protected async Task RefreshDataModal()
        {
            if (FriendPopUp != null)
                await FriendPopUp.RefreshData();
        }

        protected void NavigateToManageAccountPage()
        {
            NavigationManager.NavigateTo("/manageProfile/myAccount");
        }

        protected void NavigateToFavoriteGamingPlatforms()
        {
            NavigationManager.NavigateTo("/myFavorites/gamingPlatforms");
        }

        protected void NavigateToFavoriteGameGenres()
        {
            NavigationManager.NavigateTo("/myFavorites/gameGenres");
        }

        protected void NavigateToFavoriteGames()
        {
            NavigationManager.NavigateTo("/myFavorites/games");
        }
    }
}
