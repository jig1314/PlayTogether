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

        public List<string> FriendUserIds { get; set; }

        public List<FriendRequestDto> ActiveSentFriendRequests { get; set; }

        public List<string> ActiveSentFriendRequestIds { get; set; }

        public List<FriendRequestDto> ActiveReceivedFriendRequests { get; set; }

        public List<string> ActiveReceivedFriendRequestIds { get; set; }

        public string IdUser { get; set; }

        public BSModal UserFriendPopUpModal { get; set; }

        public UserFriendPopUp UserFriendPopUp { get; set; }

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
            IdUser = AuthenticationState.User.FindFirst("sub").Value;

            UserProfileDto = await UserService.GetUserProfileInformation(UserName);

            FriendUserIds = (await UserService.GetFriends()).Select(user => user.UserId).ToList();
            var activeFriendRequests = await UserService.GetActiveFriendRequests();

            ActiveSentFriendRequests = activeFriendRequests.Where(request => request.FromUserId == IdUser).ToList();
            ActiveSentFriendRequestIds = ActiveSentFriendRequests.Select(request => request.ToUserId).ToList();

            ActiveReceivedFriendRequests = activeFriendRequests.Where(request => request.ToUserId == IdUser).ToList();
            ActiveReceivedFriendRequestIds = ActiveReceivedFriendRequests.Select(request => request.FromUserId).ToList();

            RetrievingData = false;
        }

        protected async Task SendFriendRequest(string toUserId)
        {
            var newFriendRequest = new FriendRequestDto()
            {
                FromUserId = IdUser,
                ToUserId = toUserId
            };

            ActiveSentFriendRequestIds.Add(toUserId);
            ActiveSentFriendRequests.Add(newFriendRequest);

            await UserService.SendFriendRequest(newFriendRequest);
        }

        protected async Task CancelFriendRequest(string toUserId)
        {
            var cancelledFriendRequest = ActiveSentFriendRequests.FirstOrDefault(request => request.ToUserId == toUserId);

            ActiveSentFriendRequestIds.Remove(toUserId);
            ActiveSentFriendRequests.Remove(cancelledFriendRequest);

            await UserService.CancelFriendRequest(cancelledFriendRequest);
        }

        protected async Task AcceptFriendRequest(string fromUserId)
        {
            var acceptedFriendRequest = ActiveReceivedFriendRequests.FirstOrDefault(request => request.FromUserId == fromUserId);
            FriendUserIds.Add(fromUserId);

            await UserService.AcceptFriendRequest(acceptedFriendRequest);
        }

        protected async Task DeclineFriendRequest(string fromUserId)
        {
            var declinedFriendRequest = ActiveReceivedFriendRequests.FirstOrDefault(request => request.FromUserId == fromUserId);

            ActiveReceivedFriendRequestIds.Remove(fromUserId);
            ActiveReceivedFriendRequests.Remove(declinedFriendRequest);

            await UserService.DeclineFriendRequest(declinedFriendRequest);
        }

        protected async Task UnfriendUser(string idUser)
        {
            FriendUserIds.Remove(idUser);

            await UserService.UnfriendUser(idUser);
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
            if (UserFriendPopUp != null)
                await UserFriendPopUp.RefreshData(UserName);
        }

    }
}
