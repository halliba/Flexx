#if DEBUG
using System;
using Flexx.Wpf.ViewModels.Abstractions;

namespace Flexx.Wpf.ViewModels.Mock
{
    internal class MockMessageViewModel : ViewModel, IMessageViewModel
    {
        public string Content { get; } = "Hello World!";
        public bool IsMine { get; } = true;
        public bool IsSend { get; set; } = true;
        public IChatPartnerViewModel Receipient { get; }
        public IChatPartnerViewModel Sender { get; }
        public DateTime TimeStamp { get; } = DateTime.Now;
    }
}
#endif