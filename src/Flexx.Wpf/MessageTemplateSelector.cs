using System.Windows;
using System.Windows.Controls;
using Flexx.Wpf.ViewModels;
using Flexx.Wpf.ViewModels.Abstractions;

namespace Flexx.Wpf
{
    internal class MessageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate MineTemplate { get; set; }
        public DataTemplate OppositeTemplate { get; set; }
        public DataTemplate StatusTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ChatStatusMessage) return StatusTemplate;
            if (!(item is IMessageViewModel viewModel)) return null;
            return viewModel.IsMine
                ? MineTemplate
                : OppositeTemplate;
        }
    }
}