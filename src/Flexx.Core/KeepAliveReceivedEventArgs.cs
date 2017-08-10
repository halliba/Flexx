using System;

namespace Flexx.Core
{
    internal class KeepAliveReceivedEventArgs : EventArgs
    {
        public ChatPartner Sender { get; }

        public KeepAliveReceivedEventArgs(ChatPartner sender)
        {
            Sender = sender;
        }
    }
}