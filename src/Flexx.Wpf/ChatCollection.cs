using System.Collections.ObjectModel;
using Flexx.Wpf.ViewModels.Abstractions;

namespace Flexx.Wpf
{
    internal class ChatCollection : ObservableCollection<IChatViewModel>
    {
    }
}