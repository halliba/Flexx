using System.Windows;
using System.Windows.Input;
using Flexx.Wpf.ViewModels;

namespace Flexx.Wpf
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void TitleBar_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void AlwaysCanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        private void MinimizeCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Normal
                ? WindowState.Maximized
                : WindowState.Normal;
        }

        private void CloseCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var visible = ChatNameBox.Visibility != Visibility.Visible ? Visibility.Visible : Visibility.Collapsed;
            ChatNameBox.Visibility = visible;
            ChatPskBox.Visibility = visible;
            ChatOpenButton.Visibility = visible;
        }

        private void ChatOpenButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ChatNameBox.Text) || string.IsNullOrWhiteSpace(ChatPskBox.Text))
                return;
            (DataContext as MainViewModel)?.EnterPublicChat(ChatNameBox.Text, ChatPskBox.Text);
        }
    }
}