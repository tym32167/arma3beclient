using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Arma3BE.Client.Infrastructure.Converters
{
    public class BisyBoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && (bool)value ? new SolidColorBrush { Color = Colors.Red } : new SolidColorBrush { Color = Colors.Transparent };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}