using System.Windows.Input;

namespace Flexx.Wpf.Commands
{
    internal static class WindowCommands
    {
        public static readonly RoutedUICommand Close = new RoutedUICommand("Close", "Close", typeof(WindowCommands));

        public static readonly RoutedUICommand Minimize = new RoutedUICommand("Minimize", "Minimize", typeof(WindowCommands));

        public static readonly RoutedUICommand Maximize = new RoutedUICommand("Maximize", "Maximize", typeof(WindowCommands));
    }
}