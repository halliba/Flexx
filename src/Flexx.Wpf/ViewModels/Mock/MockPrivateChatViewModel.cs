#if DEBUG
using Flexx.Core;
using Flexx.Core.Protocol;
using Flexx.Wpf.ViewModels.Abstractions;

namespace Flexx.Wpf.ViewModels.Mock
{
    internal class MockPrivateChatViewModel : MockChatViewModel, IPrivateChatViewModel
    {
        public IChatPartnerViewModel ChatPartner { get; }
        public void NewIncomingMessage(Message message, ChatPartner partner)
        {

        }
    }
}
#endif