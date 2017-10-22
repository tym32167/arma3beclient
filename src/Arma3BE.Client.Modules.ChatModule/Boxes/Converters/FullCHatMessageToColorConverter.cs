using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Arma3BE.Client.Modules.ChatModule.Models;

namespace Arma3BE.Client.Modules.ChatModule.Boxes.Converters
{
    public class FullCHatMessageToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var message = value as FullChatMessage;
            if (message == null) return Brushes.Black;


            var color = ServerMonitorChatViewModel.GetMessageColor(message.Message);
            var brush = new SolidColorBrush(color);
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}