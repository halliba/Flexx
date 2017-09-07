using System;

namespace Flexx.Wpf.ViewModels.Abstractions
{
    internal interface IMessageViewModel : IViewModel
    {
        string Content { get; }
        bool IsMine { get; }
        bool IsSend { get; set; }
        IChatPartnerViewModel Receipient { get; }
        IChatPartnerViewModel Sender { get; }
        DateTime TimeStamp { get; }
    }
}