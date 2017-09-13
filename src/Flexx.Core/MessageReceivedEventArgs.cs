using System;
using Flexx.Core.Protocol;

namespace Flexx.Core
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public Message Message { get; }

        public UserIdentity Sender { get; }

        public MessageReceivedEventArgs(UserIdentity sender, Message message)
        {
            Sender = sender;
            Message = message;
        }
    }
}