using Flexx.Wpf.ViewModels.Abstractions;

namespace Flexx.Wpf.ViewModels.Mock
{
    internal class MockPublicChatViewModel : MockChatViewModel, IPublicChatViewModel
    {
        public MockPublicChatViewModel(string name)
        {
            Name = name;
            Abbreviation = Name.Substring(0, 1).ToUpper();
        }

        public string Abbreviation { get; }
        public string Name { get; }
        public byte[] PreSharedKey { get; } = null;
    }
}