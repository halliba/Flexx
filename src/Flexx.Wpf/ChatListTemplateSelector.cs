using System.Windows;
using System.Windows.Controls;
using Flexx.Wpf.ViewModels.Abstractions;

namespace Flexx.Wpf
{
    internal class ChatListTemplateSelector : DataTemplateSelector
    {
        public DataTemplate PrivateTemplate { get; set; }
        public DataTemplate PublicTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is IPublicChatViewModel) return PublicTemplate;
            if (item is IPrivateChatViewModel) return PrivateTemplate;
            return null;
        }
    }
}