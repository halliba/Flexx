using System;
using Flexx.Wpf.ViewModels.Abstractions;

namespace Flexx.Wpf.ViewModels
{
    internal class ChatStatusMessage : ViewModel, IChatContent
    {
        public DateTime TimeStamp { get; }

        public bool IsUnread { get; set; }

        public string Message { get; }

        public ChatStatusMessage(string message)
        {
            Message = message;
            TimeStamp = DateTime.Now;
        }
    }
}