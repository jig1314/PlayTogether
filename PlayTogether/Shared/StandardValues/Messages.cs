﻿using System;
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
        /// Name of the Hub method to register a new user
        /// </summary>
        public const string REGISTER = "Register";

        /// <summary>
        /// Name of the Hub method to send a message to an individual
        /// </summary>
        public const string SEND_DIRECT_MESSAGE = "SendDirectMessage";

        /// <summary>
        /// Name of the Hub method to send a message to Group
        /// </summary>
        public const string SEND_GROUP_MESSAGE = "SendGroupMessage";
    }
}
