using System;
using System.Windows.Threading;
using Flexx.Core;

namespace Flexx.Wpf.ViewModels
{
    internal class PublicChatViewModel : ChatViewModel
    {
        private readonly PublicChatRoom _chatRoom;

        private readonly ChatPartner _self;

        public string Name => _chatRoom.Name;

        public string Abbreviation => _chatRoom.Name.Substring(0, 1).ToUpper();

        public PublicChatViewModel(PublicChatRoom chatRoom, ChatPartner self)
        {
            _chatRoom = chatRoom;
            chatRoom.MessageReceived += NewIncomingMessage;
            _self = self;
        }

        protected override async void SendMessage(string message)
        {
            var viewModel = new MessageViewModel(true, _self, ChatPartner.Public, message, DateTime.Now);
            Messages.Add(viewModel);
            LastActivity = DateTime.Now;
            await _chatRoom.SendMessageAsync(message);
            viewModel.IsSend = true;
        }

        private void NewIncomingMessage(object sender, MessageReceivedEventArgs args)
        {
            if (args.Sender.Identity.Equals(_self.Identity))
                return;

            var viewModel = new MessageViewModel(false, args.Sender, _self, args.Message.Content, DateTime.Now) {IsSend = true};
            LastActivity = DateTime.Now;
            Dispatcher.CurrentDispatcher.Invoke(() => Messages.Add(viewModel));
        }
    }
}