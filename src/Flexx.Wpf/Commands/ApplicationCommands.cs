using System.Windows.Input;

namespace Flexx.Wpf.Commands
{
    internal static class ApplicationCommands
    {
        public static readonly RoutedUICommand ShowPublicKey
            = new RoutedUICommand(nameof(ShowPublicKey), nameof(ShowPublicKey), typeof(ApplicationCommands));
        public static readonly RoutedUICommand BeginInvite
            = new RoutedUICommand(nameof(BeginInvite), nameof(BeginInvite), typeof(ApplicationCommands));
    }
}