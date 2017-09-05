using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Flexx.Wpf.Converter
{
    [ValueConversion(typeof(Color), typeof(SolidColorBrush))]
    internal class ColorToSolidColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is Color color)
                return new SolidColorBrush(color);
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush brush)
                return brush.Color;
            return null;
        }
    }
}