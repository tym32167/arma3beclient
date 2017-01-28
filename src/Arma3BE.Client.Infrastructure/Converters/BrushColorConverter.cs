using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Arma3BE.Client.Infrastructure.Converters
{
    [ValueConversion(typeof(bool), typeof(SolidColorBrush))]
    // ReSharper disable once UnusedMember.Global
    public class BrushColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && (bool)value)
            {
                {
                    return new SolidColorBrush(Colors.Black);
                }
            }
            return new SolidColorBrush(Colors.Red);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}