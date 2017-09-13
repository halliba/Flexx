//#if DEBUG
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Flexx.Wpf.ViewModels.Abstractions;

namespace Flexx.Wpf.ViewModels.Mock
{
    internal abstract class MockChatViewModel : ViewModel, IChatViewModel
    {
        public DateTime LastActivity { get; set; } = DateTime.Now.AddMinutes(new Random().Next(0, 61));
        public string MessageDraft { get; set; } = "Hello World!";
        public ObservableCollection<IChatContent> Contents { get; } = new ObservableCollection<IChatContent>();
        public int UnreadMessageCount { get; } = 5;
        public bool HasUnreadMessages { get; } = true;
        public bool BeeingRead { get; set; } = false;

        public ICommand SendMessageCommand { get; } = null;

        public bool Equals(string name, string password) => false;
        public bool Equals(string name, byte[] preSharedKey) => false;

        public void MarkAsRead()
        {
            
        }
    }
}
//#endif