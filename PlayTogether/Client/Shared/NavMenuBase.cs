using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using PlayTogether.Client.ChatClient;
using PlayTogether.Client.Services;
using PlayTogether.Shared.DTOs;
using PlayTogether.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlayTogether.Client.Shared
{
    public class NavMenuBase : ComponentBase
    {
        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        public AuthenticationState AuthenticationState { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IMessageService MessageService { get; set; }

        public bool collapseNavMenu { get; set; } = true;

        public bool expandDirectMessages { get; set; } = false;

        public string NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        private ChatClient.ChatClient _chatClient;

        [CascadingParameter]
        public ChatClient.ChatClient ChatClient
        {
            get
            {
                return _chatClient;
            }
            set
            {
                if (_chatClient != value)
                {
                    if (_chatClient != null)
                    {
                        _chatClient.MessageReceived -= MessageReceived;
                        _chatClient.ConversationRead -= ConversationRead;
                    }

                    _chatClient = value;
                    if (_chatClient != null)
                    {
                        _chatClient.MessageReceived += MessageReceived;
                        _chatClient.ConversationRead += ConversationRead;
                    }

                    StateHasChanged();
                }
            }
        }

        public List<DirectMessageConversation> DirectMessageConversations { get; set; }
        public string IdUser { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                AuthenticationState = await AuthenticationStateTask;

                if (AuthenticationState.User.Identity.IsAuthenticated)
                {
                    IdUser = AuthenticationState.User.FindFirst("sub").Value;
                    await RefreshData();
                }
            }
        }

        private async Task RefreshData()
        {
            DirectMessageConversations = await MessageService.GetDirectMessageConversations();

            StateHasChanged();
        }

        /// <summary>
        /// Inbound message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (!DirectMessageConversations.Select(c => c.Id).Contains(e.Conversation))
            {
                await RefreshData();
            }
            else
            {
                if (e.FromUser != IdUser)
                {
                    var convo = DirectMessageConversations.FirstOrDefault(c => c.Id == e.Conversation);
                    convo.HasUnreadMessages = true;
                }
            }

            // Inform blazor the UI needs updating
            StateHasChanged();
        }

        private async void ConversationRead(object sender, ConversationReadEventArgs e)
        {
            if (!DirectMessageConversations.Select(c => c.Id).Contains(e.Conversation))
            {
                await RefreshData();
            }
            else
            {
                if (e.IdUser == IdUser)
                {
                    var convo = DirectMessageConversations.FirstOrDefault(c => c.Id == e.Conversation);
                    convo.HasUnreadMessages = false;
                }
            }

            // Inform blazor the UI needs updating
            StateHasChanged();
        }

        public void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }

        public void NavigateToChat(string userName)
        {
            NavigationManager.NavigateTo($"/chat/{userName}", true);
        }
    }
}
