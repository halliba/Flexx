using Flexx.Core;
using Flexx.Core.Protocol;

namespace Flexx.Wpf.ViewModels.Abstractions
{
    internal interface IPrivateChatViewModel : IChatViewModel
    {
        IChatPartnerViewModel ChatPartner { get; }

        void NewIncomingMessage(Message message, ChatPartner partner);
    }
}