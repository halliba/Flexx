namespace Flexx.Wpf.ViewModels.Abstractions
{
    internal interface IMainViewModel : IViewModel
    {
        ChatCollection Chats { get; }
        bool IsIncognitoModeEnabled { get; set; }

        IPublicChatViewModel EnterPublicChat(string name, string password);
    }
}