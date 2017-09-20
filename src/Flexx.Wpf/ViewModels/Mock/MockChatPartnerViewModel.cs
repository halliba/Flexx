#if DEBUG
using System;
using System.Windows.Media;
using Flexx.Core;
using Flexx.Wpf.ViewModels.Abstractions;

namespace Flexx.Wpf.ViewModels.Mock
{
    internal class MockChatPartnerViewModel : ViewModel, IChatPartnerViewModel
    {
        public string Abbreviation { get; } = "U";
        public Color Color { get; set; } = ChatColors.GetRandom("User");
        public string Name { get; } = "Name";
        public UserIdentity ChatPartner { get; } = null;
        public DateTime LastActivity { get; } = DateTime.Now;
        public string PublicKey { get; } = null;

        public bool Equals(UserIdentity identity)
        {
            return false;
        }
    }
}
#endif