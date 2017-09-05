using System;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Input;

namespace Flexx.Wpf.ViewModels
{
    internal abstract class ChatViewModel : ViewModel
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

        public ICommand SendMessageCommand
            => _sendMessageCommand ?? (_sendMessageCommand = new DelegateCommand(SendMessageInternal));

        private void SendMessageInternal(object obj)
        {
            if (!(obj is string message) || string.IsNullOrWhiteSpace(message)) return;
            SendMessage(message);
        }

        protected abstract void SendMessage(string message);

        protected ChatViewModel()
        {
            BindingOperations.EnableCollectionSynchronization(Messages, Messages);
        }

        public ObservableCollection<MessageViewModel> Messages { get; }
            = new ObservableCollection<MessageViewModel>();

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
    }
}