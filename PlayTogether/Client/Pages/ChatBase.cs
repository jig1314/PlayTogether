using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using PlayTogether.Client.ChatClient;
using PlayTogether.Client.Services;
using PlayTogether.Shared.DTOs;
using PlayTogether.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlayTogether.Client.Pages
{
    public class ChatBase : ComponentBase
    {
        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        public AuthenticationState AuthenticationState { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        [Inject]
        public IUserService UserService { get; set; }

        [Inject]
        public IMessageService MessageService { get; set; }

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
                    }

                    _chatClient = value;
                    if (_chatClient != null)
                    {
                        _chatClient.MessageReceived += MessageReceived;
                    }
                    StateHasChanged();
                }
            }
        }

        [Parameter]
        public string UserName { get; set; }

        public string MyUserId { get; set; }

        public string Message { get; set; }

        public UserBasicInfo MyUserProfileDto { get; set; }

        public UserBasicInfo UserProfileDto { get; set; }

        public List<MessageDto> Messages { get; set; }

        public string Conversation { get; set; }

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
                RetrievingData = true;

                MyUserId = AuthenticationState.User.FindFirst("sub").Value;
                var myUserName = AuthenticationState.User.Identity.Name;

                UserProfileDto = await UserService.GetUserBasicInformation(UserName);
                MyUserProfileDto = await UserService.GetUserBasicInformation(myUserName);
                Messages = await MessageService.GetMessages(UserProfileDto.UserId);

                Conversation = Messages.Select(m => m.ConversationId).Distinct().SingleOrDefault();
                if (Conversation != null && ChatClient != null)
                    await ReadMessages(Conversation);

                RetrievingData = false;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JSRuntime.InvokeAsync<bool>("scrollToBottom", "chatContainer");
        }

        protected async Task SendMessageAsync(string idUser)
        {
            await ChatClient.SendDirectMessageAsync(idUser, Message);
            Message = null;
        }

        /// <summary>
        /// Inbound message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Conversation))
            {
                RetrievingData = true;

                Messages = await MessageService.GetMessages(UserProfileDto.UserId);

                Conversation = Messages.Select(m => m.ConversationId).Distinct().SingleOrDefault();

                RetrievingData = false;
            }
            else
            {
                if (Conversation == e.Conversation)
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
            }
        }

        public async Task ReadMessages(string conversation)
        {
            await ChatClient.ReadMessages(conversation);
        }
    }
}
