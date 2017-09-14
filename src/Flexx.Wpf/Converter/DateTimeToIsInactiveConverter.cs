using System;
using System.Globalization;
using System.Windows.Data;

namespace Flexx.Wpf.Converter
{
    internal class DateTimeToIsInactiveConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is DateTime timeStamp)) return null;

            var difference = DateTime.Now - timeStamp;
            Console.WriteLine(difference);
            return difference > TimeSpan.FromSeconds(10);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException($"{typeof(DateTimeToIsInactiveConverter)} can only be used one-way.");
        }
    }
}