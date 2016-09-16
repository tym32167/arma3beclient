using System;
using System.Globalization;
using System.Windows.Data;
using Arma3BE.Client.Infrastructure.Extensions;

namespace Arma3BE.Client.Infrastructure.Converters
{
    [ValueConversion(typeof(DateTime), typeof(DateTime))]
    [ValueConversion(typeof(DateTime?), typeof(DateTime?))]
    public class UtcToLocalDateTimeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            if (value is DateTime?) return ((DateTime?)value).UtcToLocalFromSettings();
            if (value is DateTime) return ((DateTime) value).UtcToLocalFromSettings();
            
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}