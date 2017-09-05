using System.Windows;
using System.Windows.Controls;
using Flexx.Wpf.ViewModels;

namespace Flexx.Wpf
{
    internal class MessageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate MineTemplate { get; set; }
        public DataTemplate OppositeTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (!(item is MessageViewModel viewModel)) return null;
            return viewModel.IsMine
                ? MineTemplate
                : OppositeTemplate;
        }
    }
}