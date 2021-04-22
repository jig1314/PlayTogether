using System;
using System.Collections.Generic;
using System.Text;

namespace PlayTogether.Shared.StandardValues
{
    public static class Messages
    {
        /// <summary>
        /// Event name when a message is received
        /// </summary>
        public const string RECEIVE = "ReceiveMessage";

        /// <summary>
        /// Event name when a message is read
        /// </summary>
        public const string READ = "ReadMessage";

        /// <summary>
        /// Name of the Hub method to send a message to an individual
        /// </summary>
        public const string SEND_DIRECT_MESSAGE = "SendDirectMessage";

        /// <summary>
        /// Name of the Hub method to send a message to Group
        /// </summary>
        public const string SEND_GROUP_MESSAGE = "SendGroupMessage";

        /// <summary>
        /// Name of the Hub method to update users in Group
        /// </summary>
        public const string UPDATE_CHAT_GROUP_USERS = "UpdateUsersToGroup";

        /// <summary>
        /// Name of the Hub method to update name of Group
        /// </summary>
        public const string UPDATE_CHAT_GROUP_NAME = "UpdateGroupName";
    }
}
