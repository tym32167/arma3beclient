using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Arma3BE.Client.Infrastructure.Converters
{
    [ValueConversion(typeof(int), typeof(SolidColorBrush))]
    public class PlayerNumberToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return new SolidColorBrush(Colors.Black);
            var num = (int)value;
            if (num  < 0) return new SolidColorBrush(Colors.Brown);
            return new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}