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

        public async Task<List<DirectMessageConversation>> GetDirectMessageConversations() =>
            await httpClient.GetFromJsonAsync<List<DirectMessageConversation>>($"api/messages/directMessageConversations");

        public async Task<List<MessageDto>> GetMessages(string idUser) =>
            await httpClient.GetFromJsonAsync<List<MessageDto>>($"api/messages/{idUser}");
    }
}
