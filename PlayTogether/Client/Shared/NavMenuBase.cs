using BlazorStrap;
using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using PlayTogether.Client.ChatClient;
using PlayTogether.Client.Services;
using PlayTogether.Shared.DTOs;
using PlayTogether.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
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

        public bool expandGroupMessages { get; set; } = false;

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
                        _chatClient.UpdateGroupNameEvent -= UpdateGroupName;
                    }

                    _chatClient = value;
                    if (_chatClient != null)
                    {
                        _chatClient.MessageReceived += MessageReceived;
                        _chatClient.ConversationRead += ConversationRead;
                        _chatClient.UpdateGroupNameEvent += UpdateGroupName;
                    }

                    StateHasChanged();
                }
            }
        }

        public List<DirectMessageConversation> DirectMessageConversations { get; set; }

        public List<ChatGroupConversation> ChatGroupConversations { get; set; }

        public string IdUser { get; set; }

        public BSModal CreateChatGroupModal { get; set; }

        public ChatGroupViewModel ChatGroupViewModel { get; set; } = new ChatGroupViewModel();

        public string AddGroupErrorMessage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            AuthenticationState = await AuthenticationStateTask;

            if (AuthenticationState.User.Identity.IsAuthenticated)
            {
                IdUser = AuthenticationState.User.FindFirst("sub").Value;
                await RefreshData();
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (DirectMessageConversations == null || ChatGroupConversations == null)
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
            ChatGroupConversations = await MessageService.GetChatGroupConversations();

            StateHasChanged();
        }

        /// <summary>
        /// Inbound message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (DirectMessageConversations == null || ChatGroupConversations == null ||
                (!DirectMessageConversations.Select(c => c.Id).Contains(e.Conversation) && !ChatGroupConversations.Select(c => c.Id).Contains(e.Conversation)))
            {
                await RefreshData();
            }
            else
            {
                if (e.FromUser != IdUser)
                {
                    if (DirectMessageConversations.Select(c => c.Id).Contains(e.Conversation))
                    {
                        var convo = DirectMessageConversations.FirstOrDefault(c => c.Id == e.Conversation);
                        convo.HasUnreadMessages = true;
                    }
                    else
                    {
                        var convo = ChatGroupConversations.FirstOrDefault(c => c.Id == e.Conversation);
                        convo.HasUnreadMessages = true;
                    }
                }
            }

            // Inform blazor the UI needs updating
            StateHasChanged();
        }

        private async void ConversationRead(object sender, ConversationReadEventArgs e)
        {
            if (DirectMessageConversations == null || ChatGroupConversations == null ||
                (!DirectMessageConversations.Select(c => c.Id).Contains(e.Conversation) && !ChatGroupConversations.Select(c => c.Id).Contains(e.Conversation)))
            {
                await RefreshData();
            }
            else
            {
                if (e.IdUser == IdUser)
                {
                    if (DirectMessageConversations.Select(c => c.Id).Contains(e.Conversation))
                    {
                        var convo = DirectMessageConversations.FirstOrDefault(c => c.Id == e.Conversation);
                        convo.HasUnreadMessages = false;
                    }
                    else
                    {
                        var convo = ChatGroupConversations.FirstOrDefault(c => c.Id == e.Conversation);
                        convo.HasUnreadMessages = false;
                    }
                }
            }

            // Inform blazor the UI needs updating
            StateHasChanged();
        }

        private async void UpdateGroupName(object sender, UpdateGroupNameEventArgs e)
        {
            if (ChatGroupConversations == null || !ChatGroupConversations.Select(c => c.Id).Contains(e.Conversation))
            {
                await RefreshData();
            }
            else
            {
                var convo = ChatGroupConversations.FirstOrDefault(c => c.Id == e.Conversation);
                convo.Name = e.GroupName;
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

        public void OpenModal()
        {
            NavigationManager.NavigateTo("");
            CreateChatGroupModal.Show();
        }

        protected async Task CreateChatGroup()
        {
            AddGroupErrorMessage = null;

            try
            {
                var chatGroup = new ChatGroupDto() { GroupName = ChatGroupViewModel.GroupName };
                await MessageService.CreateNewChatGroup(chatGroup);
                NavigationManager.NavigateTo($"/chatGroup/{ChatGroupViewModel.GroupName}", true);
            }
            catch (Exception ex)
            {
                AddGroupErrorMessage = $"{ex.Message}";
            }
        }

        public void NavigateToChatGroup(string groupName)
        {
            NavigationManager.NavigateTo($"/chatGroup/{groupName}", true);
        }

    }
}

public class ChatGroupViewModel
{
    public string GroupName { get; set; }
}

public class ChatGroupValidator : AbstractValidator<ChatGroupViewModel>
{
    public ChatGroupValidator()
    {
        CascadeMode = CascadeMode.StopOnFirstFailure;

        RuleFor(x => x.GroupName)
                .NotEmpty().WithMessage("Please enter name for the chat group.")
                .Custom((groupName, context) =>
                {
                    var hasSymbols = new Regex(@"[<>#+%|]");

                    if (hasSymbols.IsMatch(groupName) || groupName.Contains(@"\") || groupName.Contains(@"/"))
                    {
                        context.AddFailure("Invalid symbol in group name. Please remove and try again!");
                    }
                });
    }
}
