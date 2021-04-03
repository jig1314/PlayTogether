using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using PlayTogether.Client.Services;
using PlayTogether.Client.ViewModels;
using PlayTogether.Shared.DTOs;
using PlayTogether.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PlayTogether.Client.Pages
{
    public class GamerSearchBase : ComponentBase
    {
        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        public AuthenticationState AuthenticationState { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IUserService UserService { get; set; }

        public GamerSearchViewModel GamerSearchViewModel { get; set; }

        public List<GamerSearchResult> Gamers { get; set; }

        public List<string> FriendUserIds { get; set; }

        public List<FriendRequestDto> ActiveSentFriendRequests { get; set; }

        public List<string> ActiveSentFriendRequestIds { get; set; }

        public List<string> ActiveReceivedFriendRequestIds { get; set; }

        public string IdUser { get; set; }

        protected override async Task OnInitializedAsync()
        {
            GamerSearchViewModel = new GamerSearchViewModel();

            AuthenticationState = await AuthenticationStateTask;

            if (!AuthenticationState.User.Identity.IsAuthenticated)
            {
                NavigationManager.NavigateTo("/login");
            }
            else
            {
                IdUser = AuthenticationState.User.FindFirst("sub").Value;

                FriendUserIds = await UserService.GetFriendUserIds();
                var activeFriendRequests = await UserService.GetActiveFriendRequests();

                ActiveSentFriendRequests = activeFriendRequests.Where(request => request.FromUserId == IdUser).ToList();
                ActiveSentFriendRequestIds = ActiveSentFriendRequests.Select(request => request.ToUserId).ToList();
                ActiveReceivedFriendRequestIds = activeFriendRequests.Where(request => request.ToUserId == IdUser).Select(request => request.FromUserId).ToList();
            }
        }

        protected async void OnSearchCriteriaChanged(ChangeEventArgs eventArgs)
        {
            if (eventArgs.Value != null)
            {
                var gamerSearchDto = new GamerSearchDto()
                {
                    SearchCriteria = GamerSearchViewModel.SearchCriteria
                };

                Gamers = await UserService.SearchForGamers(gamerSearchDto);
                StateHasChanged();
            }
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
    }
}
