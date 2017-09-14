using System;
using System.Windows.Media;
using Flexx.Core;
using Flexx.Wpf.Properties;
using Flexx.Wpf.ViewModels.Abstractions;

namespace Flexx.Wpf.ViewModels
{
    internal class ChatPartnerViewModel : ViewModel, IChatPartnerViewModel
    {
        private Color _color;
        private DateTime _lastActivity;

        public Color Color
        {
            get => _color;
            set
            {
                if (value.Equals(_color)) return;
                _color = value;
                OnPropertyChanged();
            }
        }

        public UserIdentity ChatPartner { get; private set; }

        public string Abbreviation => ChatPartner?.Name?.Substring(0,1).ToUpper();

        public DateTime LastActivity
        {
            get => _lastActivity;
            set
            {
                if (value.Equals(_lastActivity)) return;
                _lastActivity = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get => ChatPartner?.Name;
            set
            {
                if (value == ChatPartner.Name) return;
                ChatPartner = new UserIdentity(value, ChatPartner.PublicKey);
                OnPropertyChanged();
                OnPropertyChanged(Abbreviation);
                AssignColor();
            }
        }

        public string PublicKey => ChatPartner?.PublicKey;

        public ChatPartnerViewModel(UserIdentity chatPartner)
            : this(chatPartner, UserColors.GetRandom(chatPartner?.Name ?? string.Empty))
        {
        }

        private void AssignColor()
        {
            Color = UserColors.GetRandom(ChatPartner?.Name ?? string.Empty);
        }

        public ChatPartnerViewModel(UserIdentity chatPartner, Color color)
        {
            ChatPartner = chatPartner;
            Color = color;
        }

        public bool Equals(UserIdentity userIdentity) => ChatPartner.Equals(userIdentity);
    }
}