using System.Windows.Media;
using Flexx.Core;
using Flexx.Wpf.ViewModels.Abstractions;

namespace Flexx.Wpf.ViewModels
{
    internal class ChatPartnerViewModel : ViewModel, IChatPartnerViewModel
    {
        public ChatPartner ChatPartner { get; }
        private Color _color;

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

        public string Abbreviation => ChatPartner.Identity?.Name?.Substring(0,1).ToUpper();

        public ChatPartnerViewModel(ChatPartner chatPartner)
        {
            ChatPartner = chatPartner;
            Color = UserColors.GetRandom(ChatPartner.Identity?.Name ?? string.Empty);
        }

        public ChatPartnerViewModel(ChatPartner chatPartner, Color color)
        {
            Color = color;
            ChatPartner = chatPartner;
        }
    }
}