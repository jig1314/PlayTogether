using PlayTogether.Shared.DTOs;
using PlayTogether.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlayTogether.Client.Services
{
    public interface IMessageService
    {
        Task<List<MessageDto>> GetMessages(string idUser);
        Task<List<DirectMessageConversation>> GetDirectMessageConversations();
    }
}
