using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Flexx.Wpf.Properties;

namespace Flexx.Wpf.Converter
{
    [ValueConversion(typeof(DateTime), typeof(SolidColorBrush))]
    internal class DateTimeToActivityColorConverter : IValueConverter
    {
        public SolidColorBrush ActiveBrush { get; set; }

        public SolidColorBrush InactiveBrush { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is DateTime timeStamp)) return null;

            var difference = DateTime.Now - timeStamp;
            return difference > TimeSpan.FromMilliseconds(Settings.Default.UserOfflineBreakPoint)
                ? InactiveBrush
                : ActiveBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException($"{typeof(DateTimeToActivityColorConverter)} can only be used one-way.");
        }
    }
}