using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PlayTogether.Server.Data;
using PlayTogether.Server.Models;
using PlayTogether.Shared.Models;
using PlayTogether.Shared.StandardValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlayTogether.Server.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public ChatHub(ApplicationDbContext context)
            : base()
        {
            _context = context;
        }

        /// <summary>
        /// Log connection
        /// </summary>
        /// <returns></returns>
        public async override Task OnConnectedAsync()
        {
            var idUser = Context.UserIdentifier;
            var user = await _context.Users
                .Include(u => u.MessageConnections).ThenInclude(connection => connection.MessageConnection)
                .Include(u => u.Conversations)
                .SingleOrDefaultAsync(u => u.Id == idUser);

            if (user.MessageConnections.Any(connection => connection.MessageConnectionId == Context.ConnectionId))
            {
                var existingConnection = user.MessageConnections.Select(connection => connection.MessageConnection)
                    .FirstOrDefault(connection => connection.ConnectionId == Context.ConnectionId);

                existingConnection.Connected = true;
                _context.Entry(existingConnection).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            else
            {
                user.MessageConnections.Where(connection => connection.MessageConnection.Connected)
                    .Select(connection => connection.MessageConnection).ToList()
                    .ForEach(connection =>
                    {
                        connection.Connected = false;
                        _context.Entry(connection).State = EntityState.Modified;
                    });

                var newConnection = new MessageConnection()
                {
                    ConnectionId = Context.ConnectionId,
                    UserAgent = Context.GetHttpContext().Request.Headers["User-Agent"],
                    Connected = true
                };

                var newUserConnection = new ApplicationUser_MessageConnection()
                {
                    ApplicationUserId = user.Id,
                    MessageConnectionId = Context.ConnectionId
                };

                _context.MessageConnections.Add(newConnection);
                _context.ApplicationUser_MessageConnections.Add(newUserConnection);
                await _context.SaveChangesAsync();
            }

            foreach (var conversation in user.Conversations)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, conversation.ConversationId);
            }

            await base.OnConnectedAsync();
        }

        public async Task SendDirectMessage(string toUserId, string message)
        {
            var idUser = Context.UserIdentifier;

            var existingConversation = await _context.Conversations
                                        .Include(c => c.Users)
                                        .Where(c => c.Users.Count == 2 
                                            && c.Users.Select(c => c.ApplicationUserId).Contains(idUser) 
                                            && c.Users.Select(c => c.ApplicationUserId).Contains(toUserId)).SingleOrDefaultAsync();

            if (existingConversation == null)
            {
                var groupGuid = Guid.NewGuid().ToString();
                var toUser = await _context.Users.Include(u => u.MessageConnections).ThenInclude(connection => connection.MessageConnection)
                                .SingleOrDefaultAsync(u => u.Id == toUserId);

                var conversation = new Conversation() { Id = groupGuid };
                _context.Conversations.Add(conversation);

                var userConversations = new List<ApplicationUser_Conversation>()
                {
                    new ApplicationUser_Conversation() { ApplicationUserId = idUser, ConversationId = groupGuid },
                    new ApplicationUser_Conversation() { ApplicationUserId = toUserId, ConversationId = groupGuid }
                };
                _context.ApplicationUser_Conversations.AddRange(userConversations);

                await _context.SaveChangesAsync();

                await Groups.AddToGroupAsync(Context.ConnectionId, groupGuid);
                var toUserConnection = toUser.MessageConnections.SingleOrDefault(connection => connection.MessageConnection.Connected);
                if (toUserConnection != null)
                {
                    await Groups.AddToGroupAsync(toUserConnection.MessageConnectionId, groupGuid);
                }

                await SendMessage(idUser, groupGuid, message);
            }
            else
            {
                await SendMessage(idUser, existingConversation.Id, message);
            }
        }

        public async Task SendGroupMessage(string conversation, string message)
        {
            var idUser = Context.UserIdentifier;
            await SendMessage(idUser, conversation, message);
        }

        /// <summary>
        /// Send a message to other clients
        /// </summary>
        /// <param name="fromUser"></param>
        /// <param name="conversation"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task SendMessage(string fromUser, string conversation, string message)
        {
            var date = DateTime.Now;
            
            var newMessage = new Message()
            {
                FromUserId = fromUser,
                ConversationId = conversation,
                MessageText = message,
                DateSubmitted = date
            };

            _context.Messages.Add(newMessage);

            await _context.ApplicationUser_Conversations
                .Where(mapping => mapping.ConversationId == conversation && mapping.ApplicationUserId != fromUser)
                .ForEachAsync(mapping => 
                {
                    mapping.HasUnreadMessages = true;
                    _context.Entry(mapping).State = EntityState.Modified;
                });

            await _context.SaveChangesAsync();

            await Clients.Group(conversation).SendAsync(Messages.RECEIVE, fromUser, conversation, message, date);
        }

        public async Task ReadMessage(string conversation)
        {
            var idUser = Context.UserIdentifier;

            var userConversation = await _context.ApplicationUser_Conversations
                .SingleOrDefaultAsync(mapping => mapping.ApplicationUserId == idUser && mapping.ConversationId == conversation);

            userConversation.HasUnreadMessages = false;
            _context.Entry(userConversation).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            await Clients.Group(conversation).SendAsync(Messages.READ, idUser, conversation);
        }

        /// <summary>
        /// Log disconnection
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception e)
        {
            var idUser = Context.UserIdentifier;
            var user = await _context.Users
                .Include(u => u.MessageConnections)
                .ThenInclude(connection => connection.MessageConnection)
                .SingleOrDefaultAsync(u => u.Id == idUser);

            var existingConnection = user.MessageConnections.Select(connection => connection.MessageConnection)
                    .FirstOrDefault(connection => connection.ConnectionId == Context.ConnectionId);

            existingConnection.Connected = false;
            _context.Entry(existingConnection).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            await base.OnDisconnectedAsync(e);
        }
    }
}
