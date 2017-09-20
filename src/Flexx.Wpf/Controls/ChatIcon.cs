using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Flexx.Wpf.Controls
{
    public class ChatIcon : ContentControl
    {
        public static readonly DependencyProperty DarkForegroundColorProperty;
        public static readonly DependencyProperty LightForegroundColorProperty;
        public static readonly DependencyProperty DefaultForegroundColorProperty;

        private static readonly DependencyPropertyKey ComputedForegroundBrushPropertyKey;
        public static readonly DependencyProperty ComputedForegroundBrushProperty;

        public static readonly DependencyProperty AbbreviationProperty;
        public static readonly DependencyProperty ColorProperty;


        static ChatIcon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChatIcon),
                new FrameworkPropertyMetadata(typeof(ChatIcon)));

            AbbreviationProperty = DependencyProperty.Register(nameof(Abbreviation), typeof(string),
                typeof(ChatIcon));

            ColorProperty = DependencyProperty.Register(nameof(Color), typeof(Color),
                typeof(ChatIcon), new FrameworkPropertyMetadata(ComputedForegroundChanged));

            DarkForegroundColorProperty = DependencyProperty.Register(nameof(DarkForegroundColor), typeof(Color),
                typeof(ChatIcon));

            LightForegroundColorProperty = DependencyProperty.Register(nameof(LightForegroundColor), typeof(Color),
                typeof(ChatIcon));

            DefaultForegroundColorProperty = DependencyProperty.Register(nameof(DefaultForegroundColor), typeof(Color),
                typeof(ChatIcon));

            ComputedForegroundBrushPropertyKey = DependencyProperty.RegisterReadOnly(nameof(ComputedForegroundBrush),
                typeof(SolidColorBrush),
                typeof(ChatIcon), new PropertyMetadata(new SolidColorBrush(Colors.Black)));
            ComputedForegroundBrushProperty = ComputedForegroundBrushPropertyKey.DependencyProperty;
        }

        private static void ComputedForegroundChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (!(dependencyObject is ChatIcon chatIcon))
                return;

            var bright = PerceivedBrightness(chatIcon.Color);

            var brush = new SolidColorBrush(bright > 130 ? chatIcon.DarkForegroundColor : chatIcon.LightForegroundColor);
            chatIcon.ComputedForegroundBrush = brush;
        }

        public string Abbreviation
        {
            get => (string) GetValue(AbbreviationProperty);
            set => SetValue(AbbreviationProperty, value);
        }

        public Color Color
        {
            get => (Color) GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public Color DarkForegroundColor
        {
            get => (Color)GetValue(DarkForegroundColorProperty);
            set => SetValue(DarkForegroundColorProperty, value);
        }

        public Color LightForegroundColor
        {
            get => (Color)GetValue(LightForegroundColorProperty);
            set => SetValue(LightForegroundColorProperty, value);
        }

        public Color DefaultForegroundColor
        {
            get => (Color)GetValue(DefaultForegroundColorProperty);
            set => SetValue(DefaultForegroundColorProperty, value);
        }

        public SolidColorBrush ComputedForegroundBrush
        {
            get => (SolidColorBrush) GetValue(ComputedForegroundBrushProperty);
            private set => SetValue(ComputedForegroundBrushPropertyKey, value);
        }

        private static int PerceivedBrightness(Color c)
        {
            return (int)Math.Sqrt(
                c.R * c.R * .299 +
                c.G * c.G * .587 +
                c.B * c.B * .114);
        }
    }
}