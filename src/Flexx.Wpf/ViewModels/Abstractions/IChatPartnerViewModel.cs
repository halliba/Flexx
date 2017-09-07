using System.Windows.Media;
using Flexx.Core;

namespace Flexx.Wpf.ViewModels.Abstractions
{
    internal interface IChatPartnerViewModel : IViewModel
    {
        string Abbreviation { get; }
        ChatPartner ChatPartner { get; }
        Color Color { get; set; }
    }
}