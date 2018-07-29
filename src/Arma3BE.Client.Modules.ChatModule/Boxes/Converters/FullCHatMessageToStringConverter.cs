using System;
using System.Globalization;
using System.Windows.Data;

namespace Arma3BE.Client.Modules.ChatModule.Boxes.Converters
{
    public class FullCHatMessageToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var message = value as FullChatMessage;
            if (message == null) return string.Empty;


            return message.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}