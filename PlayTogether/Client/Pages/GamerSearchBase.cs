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

        protected override async Task OnInitializedAsync()
        {
            GamerSearchViewModel = new GamerSearchViewModel();

            AuthenticationState = await AuthenticationStateTask;

            if (!AuthenticationState.User.Identity.IsAuthenticated)
            {
                NavigationManager.NavigateTo("/login");
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

    }
}
