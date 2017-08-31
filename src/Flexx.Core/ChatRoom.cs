using System;

namespace Flexx.Core
{
    public abstract class ChatRoom
    {
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        
        protected void OnMessageReceived(MessageReceivedEventArgs e)
        {
            MessageReceived?.Invoke(this, e);
        }
    }
}