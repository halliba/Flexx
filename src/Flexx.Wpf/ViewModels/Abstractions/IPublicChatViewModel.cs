namespace Flexx.Wpf.ViewModels.Abstractions
{
    internal interface IPublicChatViewModel : IChatViewModel
    {
        string Abbreviation { get; }
        string Name { get; }
        byte[] PreSharedKey { get; }

        bool Equals(string name, string password);
        bool Equals(string name, byte[] preSharedKey);
    }
}