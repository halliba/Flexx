using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Flexx.Core;
using Flexx.Core.Utils;
using Flexx.Wpf.Commands;
using Flexx.Wpf.Properties;
using Flexx.Wpf.ViewModels.Abstractions;

namespace Flexx.Wpf.ViewModels
{
    internal class MainViewModel : ViewModel, IMainViewModel
    {
        private readonly PersonalIdentity _identity = GetIdentity();
        private readonly ChatPartnerViewModel _self;
        private readonly ChatApplication _chatApp;
        private DelegateCommand<SendInviteCommandArgs> _sendInviteCommand;

        public ChatCollection Chats { get; } = new ChatCollection();

        public ObservableCollection<ChatPartnerViewModel> ChatPartners { get; } = new ObservableCollection<ChatPartnerViewModel>();

        public DelegateCommand<SendInviteCommandArgs> SendInviteCommand => _sendInviteCommand
            ?? (_sendInviteCommand = new DelegateCommand<SendInviteCommandArgs>(args => args.Chat.SendInvite(args.User)));
        
        public bool IsIncognitoModeEnabled
        {
            get => !_chatApp.SendKeepAlive;
            set
            {
                if (value == !_chatApp.SendKeepAlive) return;
                _chatApp.SendKeepAlive = !value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            _self = new ChatPartnerViewModel(_identity);
#if DEBUG
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                Chats.Add(new PublicChatViewModel(_chatApp.EnterPublicChatRoom(), _self));
                return;
            }
#endif
            
            _chatApp = new ChatApplication(_identity);
            _chatApp.KeepAliveReceived += ChatAppOnKeepAliveReceived;
            _chatApp.InviteReceived += ChatAppOnInviteReceived;
            Chats.Add(new PublicChatViewModel(_chatApp.EnterPublicChatRoom(), _self));

            BindingOperations.EnableCollectionSynchronization(Chats, Chats);
            BindingOperations.EnableCollectionSynchronization(ChatPartners, ChatPartners);
        }

        private static PersonalIdentity GetIdentity()
        {
            var userName = Settings.Default.Username;
            if (string.IsNullOrWhiteSpace(userName))
                userName = Environment.UserName;
            var privateKey = Settings.Default.PrivateRsaKey;

            var identity = string.IsNullOrEmpty(privateKey)
                ? PersonalIdentity.Generate(userName)
                : new PersonalIdentity(userName, PemUtils.GetKeyPairFromPem(privateKey));

            Settings.Default.Username = identity.Name;
            Settings.Default.PrivateRsaKey = PemUtils.GetPemFromKeyPair(identity.KeyPair);
            Settings.Default.Save();

            return identity;
        }


        private void ChatAppOnInviteReceived(object sender, InviteReceivedEventArgs args)
        {
            if (args.Sender.Equals(_identity))
                return;

            if (Chats.OfType<IPublicChatViewModel>().Any(c => c.Equals(args.Name, args.Psk)))
                return;

            var result = MessageBox.Show(
                $"{args.Sender.Name} hat dich zum Chat {args.Name} eingeladen. Möchtest du teilnehmen?",
                "Chat-Einladung", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
                EnterPublicChat(args.Name, args.Psk);
        }

        private void ChatAppOnKeepAliveReceived(object sender, KeepAliveReceivedEventArgs args)
        {
            if (args.Sender.Equals(_identity))
                return;
            
            Application.Current?.Dispatcher.Invoke(() =>
            {
                var existing = ChatPartners.FirstOrDefault(c => c.Equals(args.Sender));
                if (existing == null)
                {
                    existing = new ChatPartnerViewModel(args.Sender) {LastActivity = DateTime.Now};
                    ChatPartners.Add(existing);
                }
                else
                {
                    existing.LastActivity = DateTime.Now;
                }
            });
        }

        public IPublicChatViewModel EnterPublicChat(string name, string password)
        {
            if (Chats.OfType<IPublicChatViewModel>().Any(c => c.Equals(name, password)))
                return null;
            var room = _chatApp.EnterPublicChatRoom(name, password);
            var viewModel = new PublicChatViewModel(room, _self);
            Chats.Add(viewModel);
            return viewModel;
        }

        public IPublicChatViewModel EnterPublicChat(string name, byte[] preSharedKey)
        {
            if (Chats.OfType<IPublicChatViewModel>().Any(c => c.Equals(name, preSharedKey)))
                return null;

            var room = _chatApp.EnterPublicChatRoom(name, preSharedKey);
            var viewModel = new PublicChatViewModel(room, _self);

            Chats.Add(viewModel);
            return viewModel;
        }
    }
}