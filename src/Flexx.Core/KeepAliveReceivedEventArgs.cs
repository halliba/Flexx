using System;

namespace Flexx.Core
{
    public class KeepAliveReceivedEventArgs : EventArgs
    {
        public UserIdentity Sender { get; }

        public KeepAliveReceivedEventArgs(UserIdentity sender)
        {
            Sender = sender;
        }
    }
}