using System;
using Flexx.Core;
using Flexx.Wpf.ViewModels.Abstractions;

namespace Flexx.Wpf.ViewModels
{
    internal class MessageViewModel : ViewModel, IMessageViewModel
    {
        private bool _isSend;
        
        public string Content { get; }

        public DateTime TimeStamp { get; }

        public bool IsSend
        {
            get => _isSend;
            set
            {
                if (value == _isSend) return;
                _isSend = value;
                OnPropertyChanged();
            }
        }

        public bool IsMine { get; }

        public IChatPartnerViewModel Sender { get; }

        public IChatPartnerViewModel Receipient { get; }

        public MessageViewModel(bool isMine, ChatPartner sender, ChatPartner receipient, string content, DateTime timeStamp)
        {
            IsMine = isMine;
            Sender = new ChatPartnerViewModel(sender);
            Receipient = new ChatPartnerViewModel(receipient);
            Content = content;
            TimeStamp = timeStamp;
        }
    }
}