namespace Flexx.Wpf.ViewModels.Abstractions
{
    internal interface IPublicChatViewModel : IChatViewModel
    {
        string Abbreviation { get; }
        string Name { get; }

        bool Equals(string name, string password);
        bool Equals(string name, byte[] preSharedKey);
    }
}