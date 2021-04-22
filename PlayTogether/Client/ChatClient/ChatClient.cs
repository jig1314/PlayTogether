using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using PlayTogether.Shared.StandardValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlayTogether.Client.ChatClient
{
    public class ChatClient : IAsyncDisposable
    {
        public const string HUBURL = "/ChatHub";

        private HubConnection _hubConnection;

        private readonly string _hubUrl;

        private readonly IAccessTokenProvider _accessTokenProvider;

        /// <summary>
        /// Flag to show if started
        /// </summary>
        private bool _started = false;

        /// <summary>
        /// Ctor: create a new client for the given hub URL
        /// </summary>
        /// <param name="siteUrl">The base URL for the site, e.g. https://localhost:1234 </param>
        /// <remarks>
        /// Changed client to accept just the base server URL so any client can use it, including ConsoleApp!
        /// </remarks>
        public ChatClient(string siteUrl, IAccessTokenProvider accessTokenProvider)
        {
            if (string.IsNullOrWhiteSpace(siteUrl))
                throw new ArgumentNullException(nameof(siteUrl));

            // set the hub URL
            _hubUrl = siteUrl.TrimEnd('/') + HUBURL;

            _accessTokenProvider = accessTokenProvider;
        }

        /// <summary>
        /// Start the SignalR client 
        /// </summary>
        public async Task StartAsync()
        {
            if (!_started)
            {
                // create the connection using the .NET SignalR client
                _hubConnection = new HubConnectionBuilder()
                    .WithUrl(_hubUrl, options =>
                    {
                        options.AccessTokenProvider = async () =>
                        {
                            var accessTokenResult = await _accessTokenProvider.RequestAccessToken();
                            accessTokenResult.TryGetToken(out var accessToken);
                            return accessToken.Value;
                        };
                    }).Build();

                // add handler for receiving messages
                _hubConnection.On<string, string, string, string, DateTime>(Messages.RECEIVE, (fromUser, fromUserFirstName, conversation, message, dateSubmitted) =>
                {
                    HandleReceiveMessage(fromUser, fromUserFirstName, conversation, message, dateSubmitted);
                });

                // add handler for receiving read receipts
                _hubConnection.On<string, string>(Messages.READ, (idUser, conversation) =>
                {
                    HandleReadConversation(idUser, conversation);
                });

                _hubConnection.Closed += async (error) =>
                {
                    _started = false;
                    await Task.Delay(new Random().Next(0, 3) * 1000);
                    await ConnectWithRetryAsync(_hubConnection);
                };

                // start the connection
                await ConnectWithRetryAsync(_hubConnection);

                _started = true;
            }
        }

        public async Task ConnectWithRetryAsync(HubConnection connection)
        {
            // Keep trying to until we can start or the token is canceled.
            while (true)
            {
                try
                {
                    await connection.StartAsync();
                    return;
                }
                catch
                {
                    // Failed to connect, trying in between 0-3 seconds.
                    await Task.Delay(new Random().Next(0, 3) * 1000);
                }
            }
        }

        /// <summary>
        /// Handle an inbound message from a hub
        /// </summary>
        /// <param name="fromUser">user who sent the message</param>
        /// <param name="fromUserFirstName">first name of user who sent the message</param>
        /// <param name="conversation">conversation the message was sent in</param>
        /// <param name="message">message content</param>
        /// <param name="dateSubmitted">date the message was sent</param>
        private void HandleReceiveMessage(string fromUser, string fromUserFirstName, string conversation, string message, DateTime dateSubmitted)
        {
            // raise an event to subscribers
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(fromUser, fromUserFirstName, conversation, message, dateSubmitted));
        }

        /// <summary>
        /// Handle an inbound message from a hub
        /// </summary>
        /// <param name="idUser">user who read the message</param>
        /// <param name="conversation">conversation the message was read in</param>
        private void HandleReadConversation(string idUser, string conversation)
        {
            // raise an event to subscribers
            ConversationRead?.Invoke(this, new ConversationReadEventArgs(idUser, conversation));
        }

        /// <summary>
        /// Event raised when this client receives a message
        /// </summary>
        /// <remarks>
        /// Instance classes should subscribe to this event
        /// </remarks>
        public event MessageReceivedEventHandler MessageReceived;

        /// <summary>
        /// Event raised when this client receives a notification that a conversation has been read by a user
        /// </summary>
        /// <remarks>
        /// Instance classes should subscribe to this event
        /// </remarks>
        public event ConversationReadEventHandler ConversationRead;

        public async Task UpdateGroupMembers(string conversation, List<string> gamersInGroup)
        {
            // check we are connected
            if (!_started)
                await StartAsync();

            // send the message
            await _hubConnection.SendAsync(Messages.UPDATE_CHAT_GROUP, conversation, gamersInGroup);
        }

        /// <summary>
        /// Send a message to a conversation
        /// </summary>
        /// <param name="conversation">conversation the message is being sent to</param>
        /// <param name="message">message to send</param>
        public async Task SendGroupMessageAsync(string conversation, string message)
        {
            // check we are connected
            if (!_started)
                await StartAsync();

            // send the message
            await _hubConnection.SendAsync(Messages.SEND_GROUP_MESSAGE, conversation, message);
        }

        public async Task ReadMessages(string conversation)
        {
            // check we are connected
            if (!_started)
                await StartAsync();

            // send the message
            await _hubConnection.SendAsync(Messages.READ, conversation);
        }

        /// <summary>
        /// Stop the client (if started)
        /// </summary>
        public async Task StopAsync()
        {
            if (_started)
            {
                // disconnect the client
                await _hubConnection.StopAsync();

                // There is a bug in the mono/SignalR client that does not
                // close connections even after stop/dispose
                // see https://github.com/mono/mono/issues/18628
                // this means the demo won't show "xxx left the chat" since 
                // the connections are left open
                await _hubConnection.DisposeAsync();
                _hubConnection = null;
                _started = false;
            }
        }

        public async ValueTask DisposeAsync()
        {
            await StopAsync();
        }

        public async Task SendDirectMessageAsync(string idUser, string message)
        {
            // check we are connected
            if (!_started)
                await StartAsync();

            // send the message
            await _hubConnection.SendAsync(Messages.SEND_DIRECT_MESSAGE, idUser, message);
        }
    }

    /// <summary>
    /// Delegate for the message handler
    /// </summary>
    /// <param name="sender">the SignalRclient instance</param>
    /// <param name="e">Event args</param>
    public delegate void MessageReceivedEventHandler(object sender, MessageReceivedEventArgs e);

    /// <summary>
    /// Message received argument class
    /// </summary>
    public class MessageReceivedEventArgs : EventArgs
    {
        public MessageReceivedEventArgs(string fromUserId, string fromUserFirstName, string conversation, string message, DateTime dateSubmitted)
        {
            FromUser = fromUserId;
            FromUserFirstName = fromUserFirstName;
            Conversation = conversation;
            Message = message;
            DateSubmitted = dateSubmitted;
        }

        /// <summary>
        /// Id of the user who sent the message
        /// </summary>
        public string FromUser { get; set; }

        /// <summary>
        /// First Name of the user who sent the message
        /// </summary>
        public string FromUserFirstName { get; set; }

        /// <summary>
        /// Id of the conversation the message was sent in
        /// </summary>
        public string Conversation { get; set; }

        /// <summary>
        /// Message data items
        /// </summary>
        public string Message { get; set; }

        /// <summary> Date  the message was submitted
        /// </summary>
        public DateTime DateSubmitted { get; set; }

    }

    /// <summary>
    /// Delegate for the message handler
    /// </summary>
    /// <param name="sender">the SignalRclient instance</param>
    /// <param name="e">Event args</param>
    public delegate void ConversationReadEventHandler(object sender, ConversationReadEventArgs e);

    /// <summary>
    /// Message received argument class
    /// </summary>
    public class ConversationReadEventArgs : EventArgs
    {
        public ConversationReadEventArgs(string idUser, string conversation)
        {
            IdUser = idUser;
            Conversation = conversation;
        }

        /// <summary>
        /// Id of the user who read the message
        /// </summary>
        public string IdUser { get; set; }

        /// <summary>
        /// Id of the conversation the message was read in
        /// </summary>
        public string Conversation { get; set; }
    }
}
