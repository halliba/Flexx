using System.Windows.Input;

namespace Flexx.Wpf.Commands
{
    internal static class ApplicationCommands
    {
        public static readonly RoutedUICommand ShowPublicKey
            = new RoutedUICommand(nameof(ShowPublicKey), nameof(ShowPublicKey), typeof(ApplicationCommands));
    }
}