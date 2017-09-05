using System.Windows.Media;
using Flexx.Core;

namespace Flexx.Wpf.ViewModels
{
    internal class ChatPartnerViewModel : ViewModel
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

        public string Abbreviation => ChatPartner.Identity?.Name?.Substring(0,1);

        public ChatPartnerViewModel(ChatPartner chatPartner)
        {
            ChatPartner = chatPartner;
        }

        public ChatPartnerViewModel(ChatPartner chatPartner, Color color)
        {
            Color = color;
            ChatPartner = chatPartner;
        }
    }
}