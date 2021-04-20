using BlazorStrap;
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
    public class ChatGroupBase : ComponentBase
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
        public string GroupName { get; set; }

        public string MyUserId { get; set; }

        public string Message { get; set; }

        public UserBasicInfo MyUserProfileDto { get; set; }

        public List<MessageDto> Messages { get; set; }

        public ChatGroupConversation Conversation { get; set; }

        public bool RetrievingData { get; set; } = false;

        public string ErrorMessage { get; set; }

        public BSModal AddGamerPopUpModal { get; set; }

        public AddGamerPopUp AddGamerPageForModal { get; set; }

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

        private async Task RefreshData(int? delayInMilliseconds = null)
        {
            RetrievingData = true;

            if (delayInMilliseconds.HasValue)
                await Task.Delay(delayInMilliseconds.Value);

            try
            {
                Messages = await MessageService.GetGroupMessages(GroupName);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            MyUserId = AuthenticationState.User.FindFirst("sub").Value;
            var myUserName = AuthenticationState.User.Identity.Name;
            MyUserProfileDto = await UserService.GetUserBasicInformation(myUserName);

            Conversation = await MessageService.GetChatGroupConversation(GroupName);
            await ReadMessages(Conversation.Id);

            RetrievingData = false;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JSRuntime.InvokeAsync<bool>("scrollToBottom", "chatContainer");
        }

        protected async Task SendGroupMessageAsync()
        {
            await ChatClient.SendGroupMessageAsync(Conversation.Id, Message);
            Message = null;
        }

        /// <summary>
        /// Inbound message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (Conversation.Id == e.Conversation)
            {
                var newMsg = new MessageDto()
                {
                    FromUserId = e.FromUser,
                    FromUser = new UserBasicInfo() { FirstName = e.FromUserFirstName },
                    ConversationId = e.Conversation,
                    MessageText = e.Message,
                    DateSubmitted = e.DateSubmitted
                };

                Messages.Add(newMsg);

                // Inform blazor the UI needs updating
                StateHasChanged();
            }
        }

        public async Task ReadMessages(string conversation)
        {
            await ChatClient.ReadMessages(conversation);
        }

        protected void RefreshModalData()
        {
            if (AddGamerPageForModal != null)
            {
                AddGamerPageForModal.ResetPopUp(Conversation);
            }
        }

        public async Task AddGamersToChatGroup()
        {
            if (AddGamerPageForModal != null && AddGamerPageForModal.GamersInGroup?.Count > 0)
            {
                await ChatClient.UpdateGroupMembers(Conversation.Id, AddGamerPageForModal.GamersInGroup);
                AddGamerPopUpModal.Hide();
                await RefreshData(1000);
            }
        }
    }
}
