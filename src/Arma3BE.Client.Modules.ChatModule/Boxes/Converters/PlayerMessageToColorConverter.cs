using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Arma3BE.Client.Modules.ChatModule.Boxes.Converters
{
    public class PlayerMessageToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value as PlayerMessage)?.isIn == true) return Brushes.Green;
            else return Brushes.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}