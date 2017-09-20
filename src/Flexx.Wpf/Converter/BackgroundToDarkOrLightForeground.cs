//using System;
//using System.Globalization;
//using System.Windows.Data;
//using System.Windows.Media;

//namespace Flexx.Wpf.Converter
//{
//    [ValueConversion(typeof(Color), typeof(SolidColorBrush))]
//    internal class BackgroundToDarkOrLightForeground : IValueConverter
//    {
//        public Color DarkColor { get; set; }

//        public Color LigthColor { get; set; }

//        public Color Default { get; set; }

//        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            if (value is Color color)
//                return PerceivedBrightness(color) > 130
//                    ? new SolidColorBrush(DarkColor)
//                    : new SolidColorBrush(LigthColor);
//            return new SolidColorBrush(Default);
//        }

//        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            throw new NotSupportedException($"{typeof(BackgroundToDarkOrLightForeground)} can only be used one-way.");
//        }

//        private static int PerceivedBrightness(Color c)
//        {
//            return (int)Math.Sqrt(
//                c.R * c.R * .299 +
//                c.G * c.G * .587 +
//                c.B * c.B * .114);
//        }
//    }
//}