using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Flexx.Core;
using Flexx.Wpf.Properties;

namespace Flexx.Wpf.ViewModels
{
    internal class MainViewModel : ViewModel
    {
        private readonly PersonalIdentity _identity = GetIdentity();
        private readonly ChatPartner _self;
        private readonly ChatApplication _chatApp;

        public ChatCollection Chats { get; } = new ChatCollection();

        public MainViewModel()
        {
            _self = new ChatPartner(_identity);
#if DEBUG
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                Chats.Add(new PublicChatViewModel(_chatApp.EnterPublicChatRoom(), _self));
                Chats.Add(new PrivateChatViewModel(null,
                    new ChatPartner(PersonalIdentity.Generate("User 1"))));
                Chats.Add(new PrivateChatViewModel(null,
                    new ChatPartner(PersonalIdentity.Generate("User 2")))
                {
                    LastActivity = DateTime.Now - TimeSpan.FromHours(4)
                });
                Chats.Add(new PrivateChatViewModel(null,
                    new ChatPartner(PersonalIdentity.Generate("User 3")))
                {
                    LastActivity = DateTime.Now - TimeSpan.FromHours(12)
                });
                Chats.Add(new PrivateChatViewModel(null,
                    new ChatPartner(PersonalIdentity.Generate("User 4")))
                {
                    LastActivity = DateTime.Now - TimeSpan.FromHours(1443)
                });
                return;
            }
#endif

            _chatApp = new ChatApplication(_identity);
            _chatApp.KeepAliveReceived += ChatAppOnKeepAliveReceived;
            _chatApp.PrivateMessageReceived += ChatAppOnPrivateMessageReceived;
            Chats.Add(new PublicChatViewModel(_chatApp.EnterPublicChatRoom(), _self));
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
        
        private void ChatAppOnPrivateMessageReceived(object sender, MessageReceivedEventArgs args)
        {
            if (args.Sender.Identity.Equals(_identity))
                return;

            Application.Current?.Dispatcher.Invoke(() =>
            {
                var chat = Chats.OfType<PrivateChatViewModel>().FirstOrDefault(c => c.ChatPartner.ChatPartner.Equals(args.Sender));
                if (chat != null)
                {
                    chat.NewIncomingMessage(args.Message, args.Sender);
                }
                else
                {
                    chat = new PrivateChatViewModel(_chatApp, args.Sender);
                    Chats.Add(chat);
                    chat.NewIncomingMessage(args.Message, args.Sender);
                }
            });
        }

        private void ChatAppOnKeepAliveReceived(object sender, KeepAliveReceivedEventArgs args)
        {
            if (args.Sender.Identity.Equals(_identity))
                return;
            
            Application.Current?.Dispatcher.Invoke(() =>
            {
                var chat = Chats.OfType<PrivateChatViewModel>().FirstOrDefault(c => c.ChatPartner.ChatPartner.Equals(args.Sender));
                if (chat != null)
                {
                    chat.LastActivity = DateTime.Now;
                }
                else
                {
                    Chats.Add(new PrivateChatViewModel(_chatApp, args.Sender));
                }
            });
        }

        public PublicChatViewModel EnterPublicChat(string name, string password)
        {
            var room = _chatApp.EnterPublicChatRoom(name, password);
            var viewModel = new PublicChatViewModel(room, _self);
            Chats.Add(viewModel);
            return viewModel;
        }
    }
}