using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Flexx.Wpf.Controls
{
    public class ChatIcon : ContentControl
    {
        public static readonly DependencyProperty AbbreviationProperty;
        public static readonly DependencyProperty ColorProperty;
        
        public string Abbreviation
        {
            get => (string)GetValue(AbbreviationProperty);
            set => SetValue(AbbreviationProperty, value);
        }

        public Color Color
        {
            get => (Color) GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        static ChatIcon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChatIcon),
                new FrameworkPropertyMetadata(typeof(ChatIcon)));

            AbbreviationProperty = DependencyProperty.Register(nameof(Abbreviation), typeof(string),
                typeof(ChatIcon));

            ColorProperty = DependencyProperty.Register(nameof(Color), typeof(Color),
                typeof(ChatIcon));
        }
    }
}