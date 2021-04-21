using BlazorStrap;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using PlayTogether.Client.ChatClient;
using PlayTogether.Shared.DTOs;
using PlayTogether.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlayTogether.Client.Shared
{
    public class MainLayoutBase : LayoutComponentBase
    {
        private Task<AuthenticationState> _authenticationStateTask;

        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationStateTask
        {
            get
            {
                return _authenticationStateTask;
            }
            set
            {
                if (_authenticationStateTask != value)
                {
                    _authenticationStateTask = value;
                    if (_authenticationStateTask != null)
                    {
                        UpdateAuthenticationState();
                    }
                }
            }
        }

        public AuthenticationState AuthenticationState { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IAccessTokenProvider AccessTokenProvider { get; set; }

        [Inject]
        public IBootstrapCss BootstrapCss { get; set; }

        public ChatClient.ChatClient ChatClient { get; set; }

        public string Message { get; set; }

        public List<MessageDto> Messages { get; private set; } = new List<MessageDto>();

        /// <summary> Implementing bootstrp for the Blazor Framework
        /// See this link for more info: https://blazorstrap.io/
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {
            await BootstrapCss.SetBootstrapCss(Theme.Cosmo, "4.3.1");
        }

        private async void UpdateAuthenticationState()
        {
            AuthenticationState = await _authenticationStateTask;

            if (AuthenticationState.User.Identity.IsAuthenticated)
            {
                await InitializeChatClient();
                StateHasChanged();
            }
        }

        /// <summary>
        /// Start chat client
        /// </summary>
        public async Task InitializeChatClient()
        {
            // Create the chat client
            string baseUrl = NavigationManager.BaseUri;
            ChatClient = new ChatClient.ChatClient(baseUrl, AccessTokenProvider);

            // add an event handler for incoming messages
            ChatClient.MessageReceived += MessageReceived;

            // start the client
            await ChatClient.StartAsync();
        }

        /// <summary>
        /// Inbound message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            var newMsg = new MessageDto()
            {
                FromUserId = e.FromUser,
                ConversationId = e.Conversation,
                MessageText = e.Message,
                DateSubmitted = e.DateSubmitted
            };

            Messages.Add(newMsg);

            // Inform blazor the UI needs updating
            StateHasChanged();
        }

        public async Task DisconnectAsync()
        {
            await ChatClient.StopAsync();
            ChatClient = null;
        }
    }
}
