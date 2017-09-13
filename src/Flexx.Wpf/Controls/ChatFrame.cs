using System.Windows;
using System.Windows.Controls;
using Flexx.Wpf.ViewModels.Abstractions;

namespace Flexx.Wpf.Controls
{
    public class ChatFrame : ContentPresenter
    {
        static ChatFrame()
        {
            ContentProperty.OverrideMetadata(typeof(ChatFrame),
                new FrameworkPropertyMetadata(OnContentChanged));
        }

        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is IChatViewModel oldModel)
                oldModel.BeeingRead = false;

            if (!(e.NewValue is IChatViewModel newModel)) return;
            newModel.BeeingRead = true;
            newModel.MarkAsRead();
        }
    }
}