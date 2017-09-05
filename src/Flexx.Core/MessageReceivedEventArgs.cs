﻿using System;
using Flexx.Core.Protocol;

namespace Flexx.Core
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public Message Message { get; }

        public ChatPartner Sender { get; }

        public MessageReceivedEventArgs(ChatPartner sender, Message message)
        {
            Sender = sender;
            Message = message;
        }
    }
}