using System;
using Flexx.Core;
using Flexx.Core.Protocol;

namespace Flexx.Wpf.ViewModels
{
    internal class PrivateChatViewModel : ChatViewModel
    {
        private readonly ChatApplication _chatApp;

        public ChatPartnerViewModel ChatPartner { get; }
        
        public PrivateChatViewModel(ChatApplication chatApp, ChatPartner chatPartner)
        {
            _chatApp = chatApp;
            ChatPartner = new ChatPartnerViewModel(chatPartner, UserColors.GetRandom(chatPartner.Identity.GetHashCode()));
        }

        protected override async void SendMessage(string message)
        {
            var viewModel = new MessageViewModel(true, _chatApp.Me, ChatPartner.ChatPartner, message, DateTime.Now);
            Messages.Add(viewModel);
            await _chatApp.SendPrivateMessageAsync(message, ChatPartner.ChatPartner);
            viewModel.IsSend = true;
        }

        public void NewIncomingMessage(Message message, ChatPartner partner)
        {
            var viewModel = new MessageViewModel(false, partner, _chatApp.Me, message.Content, DateTime.Now) { IsSend = true };
            LastActivity = DateTime.Now;
            Messages.Add(viewModel);
        }
    }
}