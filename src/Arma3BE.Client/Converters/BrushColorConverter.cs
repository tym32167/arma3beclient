using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Arma3BEClient.Converters
{
    [ValueConversion(typeof(bool), typeof(SolidColorBrush))]
    public class BrushColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                {
                    return new SolidColorBrush(System.Windows.Media.Colors.Black);
                }
            }
            return new SolidColorBrush(System.Windows.Media.Colors.Red);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}