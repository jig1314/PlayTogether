using PlayTogether.Shared.DTOs;
using PlayTogether.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace PlayTogether.Client.Services
{
    public class MessageService : IMessageService
    {
        private readonly HttpClient httpClient;

        public MessageService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task CreateNewChatGroup(ChatGroupDto chatGroup)
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync("api/messages/createChatGroup", chatGroup);
                var content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    throw new ApplicationException(content);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ChatGroupConversation> GetChatGroupConversation(string groupName) =>
            await httpClient.GetFromJsonAsync<ChatGroupConversation>($"api/messages/chatGroupConversation/{groupName}");

        public async Task<List<ChatGroupConversation>> GetChatGroupConversations() =>
            await httpClient.GetFromJsonAsync<List<ChatGroupConversation>>($"api/messages/chatGroupConversations");

        public async Task<DirectMessageConversation> GetDirectMessageConversation(string idUser) =>
            await httpClient.GetFromJsonAsync<DirectMessageConversation>($"api/messages/directMessageConversation/{idUser}");

        public async Task<List<DirectMessageConversation>> GetDirectMessageConversations() =>
            await httpClient.GetFromJsonAsync<List<DirectMessageConversation>>($"api/messages/directMessageConversations");

        public async Task<List<MessageDto>> GetGroupMessages(string groupName)
        {
            try
            {
                return await httpClient.GetFromJsonAsync<List<MessageDto>>($"api/messages/groupMessages/{groupName}");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<MessageDto>> GetMessages(string idUser) =>
            await httpClient.GetFromJsonAsync<List<MessageDto>>($"api/messages/{idUser}");
    }
}
