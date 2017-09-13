using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Flexx.Wpf.ViewModels;
using Flexx.Wpf.ViewModels.Abstractions;

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

        private void ChatFrame_OnDrop(object sender, DragEventArgs e)
        {
            if (!(sender is ContentPresenter presenter))
                return;
            if (!(presenter.Content is PublicChatViewModel chatViewModel))
                return;

            var formats = e.Data.GetFormats();
            if (!e.Data.GetFormats().Contains("InviteFormat")
                || !(e.Data.GetData("InviteFormat") is IChatPartnerViewModel user))
                return;
            
            chatViewModel.SendInvite(user.ChatPartner);
        }

        private void ChatPartnerList_OnMouseMove(object sender, MouseEventArgs e)
        {
            OnMouseMove(e);
            if (e.LeftButton != MouseButtonState.Pressed) return;
            if (!(sender is ListView listView)) return;

            var data = new DataObject();
            data.SetData("InviteFormat", listView.SelectedItem);
            
            DragDrop.DoDragDrop(this, data, DragDropEffects.Copy | DragDropEffects.Move);
        }
    }
}