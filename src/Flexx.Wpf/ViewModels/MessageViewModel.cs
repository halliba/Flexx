using System;
using Flexx.Core;

namespace Flexx.Wpf.ViewModels
{
    internal class MessageViewModel : ViewModel
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

        public ChatPartner Sender { get; }

        public ChatPartner Receipient { get; }

        public MessageViewModel(bool isMine, ChatPartner sender, ChatPartner receipient, string content, DateTime timeStamp)
        {
            IsMine = isMine;
            Sender = sender;
            Receipient = receipient;
            Content = content;
            TimeStamp = timeStamp;
        }
    }
}