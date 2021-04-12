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
    public class UserFriendPopUpBase : ComponentBase
    {
        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        public AuthenticationState AuthenticationState { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IUserService UserService { get; set; }

        public BSTab TabMutualFriends { get; set; }

        public BSTab TabFriends { get; set; }

        public List<UserBasicInfo> UsersFriends { get; set; }

        public List<string> FriendUserIds { get; set; }

        public List<FriendRequestDto> ActiveSentFriendRequests { get; set; }

        public List<string> ActiveSentFriendRequestIds { get; set; }

        public List<FriendRequestDto> ActiveReceivedFriendRequests { get; set; }

        public List<string> ActiveReceivedFriendRequestIds { get; set; }

        public string IdUser { get; set; }

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
                IdUser = AuthenticationState.User.FindFirst("sub").Value;
            }
        }

        public async Task RefreshData(string userName)
        {
            UsersFriends = (await UserService.GetUsersFriends(userName)).Where(user => user.UserId != IdUser).ToList();

            FriendUserIds = (await UserService.GetFriends()).Select(user => user.UserId).ToList();
            var activeFriendRequests = await UserService.GetActiveFriendRequests();

            ActiveSentFriendRequests = activeFriendRequests.Where(request => request.FromUserId == IdUser).ToList();
            ActiveSentFriendRequestIds = ActiveSentFriendRequests.Select(request => request.ToUserId).ToList();

            ActiveReceivedFriendRequests = activeFriendRequests.Where(request => request.ToUserId == IdUser).ToList();
            ActiveReceivedFriendRequestIds = ActiveReceivedFriendRequests.Select(request => request.FromUserId).ToList();

            StateHasChanged();
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
    }
}
