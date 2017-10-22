using System;
using System.Globalization;
using System.Windows.Data;

namespace Arma3BE.Client.Modules.ChatModule.Boxes.Converters
{
    public class ConnectTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as PlayerMessage)?.isIn == true ? "connected" : "disconnected";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}