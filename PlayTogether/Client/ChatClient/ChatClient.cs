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

        /// <summary>
        /// Id of the chatter
        /// </summary>
        private readonly string _idUser;

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
        public ChatClient(string idUser, string siteUrl, IAccessTokenProvider accessTokenProvider)
        {
            // check inputs
            if (string.IsNullOrWhiteSpace(idUser))
                throw new ArgumentNullException(nameof(idUser));

            if (string.IsNullOrWhiteSpace(siteUrl))
                throw new ArgumentNullException(nameof(siteUrl));

            // save user id
            _idUser = idUser;

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
                    })
                    .Build();

                // add handler for receiving messages
                _hubConnection.On<string, string, string>(Messages.RECEIVE, (fromUser, conversation, message) =>
                {
                    HandleReceiveMessage(fromUser, conversation, message);
                });

                // start the connection
                await _hubConnection.StartAsync();

                _started = true;
            }
        }

        /// <summary>
        /// Handle an inbound message from a hub
        /// </summary>
        /// <param name="fromUser">user who sent the message</param>
        /// <param name="conversation">conversation the message was sent in</param>
        /// <param name="message">message content</param>
        private void HandleReceiveMessage(string fromUser, string conversation, string message)
        {
            // raise an event to subscribers
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(fromUser, conversation, message));
        }

        /// <summary>
        /// Event raised when this client receives a message
        /// </summary>
        /// <remarks>
        /// Instance classes should subscribe to this event
        /// </remarks>
        public event MessageReceivedEventHandler MessageReceived;

        /// <summary>
        /// Send a message to a conversation
        /// </summary>
        /// <param name="conversation">conversation the message is being sent to</param>
        /// <param name="message">message to send</param>
        public async Task SendGroupMessageAsync(string conversation, string message)
        {
            // check we are connected
            if (!_started)
                throw new InvalidOperationException("Client not started");

            // send the message
            await _hubConnection.SendAsync(Messages.SEND_GROUP_MESSAGE, conversation, message);
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

        public async Task SendDirectMessageAsync(string idUser)
        {
            // check we are connected
            if (!_started)
                throw new InvalidOperationException("Client not started");

            // send the message
            await _hubConnection.SendAsync(Messages.SEND_DIRECT_MESSAGE, idUser, "Nice to meet you");
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
        public MessageReceivedEventArgs(string fromUser, string conversation, string message)
        {
            FromUser = fromUser;
            Conversation = conversation;
            Message = message;
        }

        /// <summary>
        /// Id of the user who sent the message
        /// </summary>
        public string FromUser { get; set; }

        /// <summary>
        /// Id of the conversation the message was sent in
        /// </summary>
        public string Conversation { get; set; }

        /// <summary>
        /// Message data items
        /// </summary>
        public string Message { get; set; }

    }
}
