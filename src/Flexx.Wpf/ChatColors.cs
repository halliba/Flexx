using System;
using System.Windows.Media;

namespace Flexx.Wpf
{
    internal static class ChatColors
    {
        public static Color GetRandom(object seed)
        {
            var random = new Random(seed.GetHashCode());
            return Colors[random.Next(0, Colors.Length)];
        }

        private static readonly Color[] Colors = {
            Color.FromRgb(33, 149, 242),
            Color.FromRgb(243, 67, 54),
            Color.FromRgb(254, 192, 7),
            Color.FromRgb(96, 125, 138),
            Color.FromRgb(232, 30, 99),
            Color.FromRgb(76, 174, 80),
            Color.FromRgb(63, 81, 180),
            Color.FromRgb(204, 219, 57)
        };
    }
}