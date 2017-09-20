using System.Collections.ObjectModel;

namespace Flexx.Wpf.ViewModels.Abstractions
{
    internal interface IMainViewModel : IViewModel
    {
        ChatCollection Chats { get; }

        ObservableCollection<ChatPartnerViewModel> ChatPartners { get; }

        DelegateCommand<IPublicChatViewModel> LeaveChatroomCommand { get; }

        bool IsIncognitoModeEnabled { get; set; }

        IPublicChatViewModel EnterPublicChat(string name, string password);

        IPublicChatViewModel EnterPublicChat(string name, byte[] password);
    }
}