using System.Windows.Media;
using Flexx.Core;

namespace Flexx.Wpf.ViewModels.Abstractions
{
    internal interface IChatPartnerViewModel : IViewModel
    {
        string Abbreviation { get; }
        Color Color { get; set; }
        string Name { get; }
        UserIdentity ChatPartner { get; }

        bool Equals(UserIdentity identity);
    }
}