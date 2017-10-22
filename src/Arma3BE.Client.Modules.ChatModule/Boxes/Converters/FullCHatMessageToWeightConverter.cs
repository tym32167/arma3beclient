using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Arma3BE.Client.Modules.ChatModule.Boxes.Converters
{
    public class FullCHatMessageToWeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var message = value as FullChatMessage;
            if (message == null) return FontWeights.Normal;

            if (message.IsImportant) return FontWeights.Heavy;

            return FontWeights.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}