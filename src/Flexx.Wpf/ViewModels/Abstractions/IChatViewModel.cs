using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Flexx.Wpf.ViewModels.Abstractions
{
    internal interface IChatViewModel : IViewModel
    {
        DateTime LastActivity { get; set; }
        string MessageDraft { get; set; }
        ObservableCollection<IChatContent> Contents { get; }
        int UnreadMessageCount { get; }
        bool HasUnreadMessages { get; }
        bool BeeingRead { get; set; }

        ICommand SendMessageCommand { get; }

        void MarkAsRead();
    }
}