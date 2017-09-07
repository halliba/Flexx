﻿#if DEBUG
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
        public ObservableCollection<IMessageViewModel> Messages { get; } = new ObservableCollection<IMessageViewModel>();
        public ICommand SendMessageCommand { get; } = null;
    }
}
#endif