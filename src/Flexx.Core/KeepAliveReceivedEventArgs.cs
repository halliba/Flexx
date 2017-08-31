using System;

namespace Flexx.Core
{
    public class KeepAliveReceivedEventArgs : EventArgs
    {
        public ChatPartner Sender { get; }

        public KeepAliveReceivedEventArgs(ChatPartner sender)
        {
            Sender = sender;
        }
    }
}