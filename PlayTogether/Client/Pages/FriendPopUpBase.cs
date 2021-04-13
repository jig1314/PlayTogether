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
    public class FriendPopUpBase : ComponentBase
    {
        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        public AuthenticationState AuthenticationState { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IUserService UserService { get; set; }

        public BSTab TabFriendRequests { get; set; }

        public BSTab TabFriends { get; set; }

        public List<UserBasicInfo> FriendUsers { get; set; }

        public List<FriendRequestDto> ActiveReceivedFriendRequests { get; set; }

        public List<string> AcceptedFriendRequestsUserIds { get; set; } = new List<string>();

        public List<string> DeclinedFriendRequestsUserIds { get; set; } = new List<string>();

        protected override async Task OnInitializedAsync()
        {
            AuthenticationState = await AuthenticationStateTask;

            if (!AuthenticationState.User.Identity.IsAuthenticated)
            {
                NavigationManager.NavigateTo($"/login/{Uri.EscapeDataString(NavigationManager.Uri)}");
            }
        }

        public async Task RefreshData()
        {
            var idUser = AuthenticationState.User.FindFirst("sub").Value;

            FriendUsers = await UserService.GetFriends();

            var activeFriendRequests = await UserService.GetActiveFriendRequests();
            ActiveReceivedFriendRequests = activeFriendRequests.Where(request => request.ToUserId == idUser).ToList();

            StateHasChanged();
        }

        protected async Task AcceptFriendRequest(string fromUserId)
        {
            var acceptedFriendRequest = ActiveReceivedFriendRequests.FirstOrDefault(request => request.FromUserId == fromUserId);
            AcceptedFriendRequestsUserIds.Add(fromUserId);

            await UserService.AcceptFriendRequest(acceptedFriendRequest);
        }

        protected async Task DeclineFriendRequest(string fromUserId)
        {
            var declinedFriendRequest = ActiveReceivedFriendRequests.FirstOrDefault(request => request.FromUserId == fromUserId);
            DeclinedFriendRequestsUserIds.Add(fromUserId);

            await UserService.DeclineFriendRequest(declinedFriendRequest);
        }
    }
}
