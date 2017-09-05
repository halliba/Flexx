using System;
using System.Globalization;
using System.Windows.Data;

namespace Flexx.Wpf.Converter
{
    internal class DateTimeToTimeAgoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is DateTime timeStamp)) return null;

            var difference = DateTime.Now - timeStamp;
            if (difference.Hours <= 1 || timeStamp.Date == DateTime.Today)
            {
                if (difference.Hours > 0)
                    return difference.Hours == 1 ? "vor einer Stunde" : $"vor {difference.Hours} Stunden";
                if(difference.Minutes > 0)
                    return difference.Minutes == 1 ? "vor einer Minute" : $"vor {difference.Minutes} Minuten";
                return "gerade eben";
            }
            if (timeStamp.Date == DateTime.Now - TimeSpan.FromDays(1))
                return "gestern";
            if (timeStamp.Date == DateTime.Now - TimeSpan.FromDays(2))
                return "vorgestern";
            if (difference.Days <= 30)
                return $"vor {difference.Days} Tagen";
            return difference.Days <= 300
                ? $"vor {difference.Days / 30} Monaten"
                : $"vor {difference.Days / 365} Jahren";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException($"{typeof(DateTimeToTimeAgoConverter)} can only be used one way.");
        }
    }
}