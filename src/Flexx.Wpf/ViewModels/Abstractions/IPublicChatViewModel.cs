namespace Flexx.Wpf.ViewModels.Abstractions
{
    internal interface IPublicChatViewModel : IChatViewModel
    {
        string Abbreviation { get; }
        string Name { get; }
    }
}