using System;
using Flexx.Core;
using Flexx.Wpf.ViewModels.Abstractions;

namespace Flexx.Wpf.ViewModels
{
    internal class MessageViewModel : ViewModel, IMessageViewModel, IChatContent
    {
        private bool _isSend;
        
        public string Content { get; }

        public DateTime TimeStamp { get; }

        public bool IsUnread { get; set; }

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

        public MessageViewModel(bool isMine, UserIdentity sender, IChatPartnerViewModel receipient, string content, DateTime timeStamp)
        {
            IsMine = isMine;
            Sender = new ChatPartnerViewModel(sender);
            Receipient = receipient;
            Content = content;
            TimeStamp = timeStamp;
        }

        public MessageViewModel(bool isMine, IChatPartnerViewModel sender, UserIdentity receipient, string content, DateTime timeStamp)
        {
            IsMine = isMine;
            Sender = sender;
            Receipient = new ChatPartnerViewModel(receipient);
            Content = content;
            TimeStamp = timeStamp;
        }
    }
}