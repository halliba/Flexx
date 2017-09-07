#if DEBUG
using System.Windows.Media;
using Flexx.Core;
using Flexx.Wpf.ViewModels.Abstractions;

namespace Flexx.Wpf.ViewModels.Mock
{
    internal class MockChatPartnerViewModel : ViewModel, IChatPartnerViewModel
    {
        public string Abbreviation { get; } = "U";
        public ChatPartner ChatPartner { get; } = new ChatPartner(new UserIdentity("User", "..."));
        public Color Color { get; set; } = UserColors.GetRandom("User");
    }
}
#endif