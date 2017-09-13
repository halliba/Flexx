﻿using System;
using System.Windows.Threading;
using Flexx.Core;
using Flexx.Wpf.ViewModels.Abstractions;

namespace Flexx.Wpf.ViewModels
{
    internal class PublicChatViewModel : ChatViewModel, IPublicChatViewModel
    {
        private readonly PublicChatRoom _chatRoom;

        private readonly IChatPartnerViewModel _self;

        public string Name => _chatRoom.Name;

        public string Abbreviation => _chatRoom.Name.Substring(0, 1).ToUpper();

        public PublicChatViewModel(PublicChatRoom chatRoom, IChatPartnerViewModel self)
        {
            _chatRoom = chatRoom;
            chatRoom.MessageReceived += NewIncomingMessage;
            _self = self;
        }

        protected override async void SendMessage(string message)
        {
            var viewModel = new MessageViewModel(true, _self, UserIdentity.Public, message, DateTime.Now);
            Messages.Add(viewModel);
            LastActivity = DateTime.Now;
            await _chatRoom.SendMessageAsync(message);
            viewModel.IsSend = true;
        }

        public async void SendInvite(UserIdentity user)
        {
            await _chatRoom.SendInviteAsync(user);
        }

        private void NewIncomingMessage(object sender, MessageReceivedEventArgs args)
        {
            if (_self.Equals(args.Sender))
                return;

            var viewModel = new MessageViewModel(false, args.Sender, _self, args.Message.Content, DateTime.Now) {IsSend = true};
            LastActivity = DateTime.Now;
            Dispatcher.CurrentDispatcher.Invoke(() => Messages.Add(viewModel));
        }
    }
}