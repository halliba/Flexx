using System;

namespace Flexx.Core
{
    public class InviteReceivedEventArgs : EventArgs
    {
        public byte[] Psk { get; }

        public string Name { get; }

        public UserIdentity Sender { get; }

        public InviteReceivedEventArgs(string name, byte[] psk, UserIdentity sender)
        {
            Name = name;
            Psk = psk;
            Sender = sender;
        }
    }
}