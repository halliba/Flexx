#if DEBUG
using Flexx.Wpf.ViewModels.Abstractions;

namespace Flexx.Wpf.ViewModels.Mock
{
    internal class MockMainViewModel : ViewModel, IMainViewModel
    {
        public ChatCollection Chats { get; } = new ChatCollection();

        public bool IsIncognitoModeEnabled { get; set; } = true;

        public MockMainViewModel()
        {
            Chats.Add(new MockPublicChatViewModel("Public"));
        }

        public IPublicChatViewModel EnterPublicChat(string name, string password)
        {
            return null;
        }
    }
}
#endif