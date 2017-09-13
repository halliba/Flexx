using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using Flexx.Wpf.ViewModels.Abstractions;

namespace Flexx.Wpf.ViewModels
{
    internal abstract class ChatViewModel : ViewModel, IChatViewModel
    {
        private DateTime _lastActivity = DateTime.Now;
        private ICommand _sendMessageCommand;
        private string _messageDraft;

        public string MessageDraft
        {
            get => _messageDraft;
            set
            {
                if (value == _messageDraft) return;
                _messageDraft = value;
                OnPropertyChanged();
            }
        }

        public bool HasUnreadMessages => UnreadMessageCount > 0;
        public bool BeeingRead { get; set; }

        public int UnreadMessageCount => Contents.Count(c => c.IsUnread);

        public ObservableCollection<IChatContent> Contents { get; }
            = new ObservableCollection<IChatContent>();

        public DateTime LastActivity
        {
            get => _lastActivity;
            set
            {
                if (value.Equals(_lastActivity)) return;
                _lastActivity = value;
                OnPropertyChanged();
            }
        }

        public ICommand SendMessageCommand
            => _sendMessageCommand ?? (_sendMessageCommand = new DelegateCommand(SendMessageInternal));

        public void MarkAsRead()
        {
            foreach (var chatContent in Contents.Where(c => c.IsUnread))
            {
                chatContent.IsUnread = false;
            }
            OnPropertyChanged(nameof(UnreadMessageCount));
            OnPropertyChanged(nameof(HasUnreadMessages));
        }

        private void SendMessageInternal(object obj)
        {
            if (!(obj is string message) || string.IsNullOrWhiteSpace(message)) return;
            SendMessage(message);
        }

        protected abstract void SendMessage(string message);

        protected ChatViewModel()
        {
            BindingOperations.EnableCollectionSynchronization(Contents, Contents);
        }
    }
}