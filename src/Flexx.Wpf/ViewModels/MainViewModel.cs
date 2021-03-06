﻿using System;
using System.Collections.ObjectModel;
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
        private DelegateCommand<IPublicChatViewModel> _leaveChatroomCommand;

        public ChatCollection Chats { get; } = new ChatCollection();

        public ObservableCollection<ChatPartnerViewModel> ChatPartners { get; } = new ObservableCollection<ChatPartnerViewModel>();

        public DelegateCommand<SendInviteCommandArgs> SendInviteCommand => _sendInviteCommand
            ?? (_sendInviteCommand = new DelegateCommand<SendInviteCommandArgs>(args => args.Chat.SendInvite(args.User)));

        public DelegateCommand<IPublicChatViewModel> LeaveChatroomCommand => _leaveChatroomCommand
            ?? (_leaveChatroomCommand = new DelegateCommand<IPublicChatViewModel>(LeaveChatroom));
        
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
            BindingOperations.EnableCollectionSynchronization(Chats, Chats);
            BindingOperations.EnableCollectionSynchronization(ChatPartners, ChatPartners);

            _self = new ChatPartnerViewModel(_identity);

            _chatApp = new ChatApplication(_identity);
            _chatApp.KeepAliveReceived += ChatAppOnKeepAliveReceived;
            _chatApp.InviteReceived += ChatAppOnInviteReceived;

            var store = ChatStore.Load();
            if (store?.Chats != null)
            {
                foreach (var storedChat in store.Chats)
                {
                    var viewModel = EnterPublicChat(storedChat.Name, storedChat.PreSharedKey, true);
                    viewModel.LastActivity = storedChat.LastActivity;
                }
            }
            if (store?.Users != null)
            {
                foreach (var storedUser in store.Users)
                {
                    ChatPartners.Add(new ChatPartnerViewModel(new UserIdentity(storedUser.Name, storedUser.PublicKey))
                    {
                        LastActivity = storedUser.LastActivity
                    });
                }
            }
            if (Chats.Count == 0)
            {
                Chats.Add(new PublicChatViewModel(_chatApp.EnterPublicChatRoom(), _self));
            }
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
                ChatStore.Store(Chats.OfType<IPublicChatViewModel>(), ChatPartners);
            });
        }

        public IPublicChatViewModel EnterPublicChat(string name, string password)
        {
            return EnterPublicChat(name, password, false);
        }

        public IPublicChatViewModel EnterPublicChat(string name, string password, bool preventSave)
        {
            if (Chats.OfType<IPublicChatViewModel>().Any(c => c.Equals(name, password)))
                return null;

            var room = _chatApp.EnterPublicChatRoom(name, password);
            var viewModel = new PublicChatViewModel(room, _self);

            Chats.Add(viewModel);

            if (preventSave) return viewModel;
            try
            {
                ChatStore.Store(Chats.OfType<IPublicChatViewModel>(), ChatPartners);
            }
            catch (Exception)
            {
                // ignored
            }

            return viewModel;
        }

        public IPublicChatViewModel EnterPublicChat(string name, byte[] preSharedKey)
        {
            return EnterPublicChat(name, preSharedKey, false);
        }

        private IPublicChatViewModel EnterPublicChat(string name, byte[] preSharedKey, bool preventSave)
        {
            if (Chats.OfType<IPublicChatViewModel>().Any(c => c.Equals(name, preSharedKey)))
                return null;

            var room = _chatApp.EnterPublicChatRoom(name, preSharedKey);
            var viewModel = new PublicChatViewModel(room, _self);

            Chats.Add(viewModel);

            if (preventSave) return viewModel;
            try
            {
                ChatStore.Store(Chats.OfType<IPublicChatViewModel>(), ChatPartners);
            }
            catch (Exception)
            {
                // ignored
            }

            return viewModel;
        }

        private void LeaveChatroom(IPublicChatViewModel chatViewModel)
        {
            if (chatViewModel == null)
                return;

            var result = MessageBox.Show($"Willst du den Chatraum '{chatViewModel.Name} wirklich verlassen?", "Chatraum verlassen?", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes)
                return;

            Chats.Remove(chatViewModel);
            chatViewModel.Leave();

            ChatStore.Store(Chats.OfType<IPublicChatViewModel>(), ChatPartners);
        }
    }
}