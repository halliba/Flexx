using Flexx.Core;
using Flexx.Wpf.ViewModels;

namespace Flexx.Wpf.Commands
{
    internal class SendInviteCommandArgs
    {
        public UserIdentity User { get; }

        public PublicChatViewModel Chat { get; }

        public SendInviteCommandArgs(UserIdentity user, PublicChatViewModel chat)
        {
            User = user;
            Chat = chat;
        }
    }
}