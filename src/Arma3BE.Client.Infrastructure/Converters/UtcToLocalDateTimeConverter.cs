using Arma3BE.Client.Infrastructure.Extensions;
using System;
using System.Globalization;
using System.Windows.Data;
// ReSharper disable HeuristicUnreachableCode

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
            // ReSharper disable once CanBeReplacedWithTryCastAndCheckForNull
            if (value is DateTime?) return ((DateTime?)value).UtcToLocalFromSettings();
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            // ReSharper disable once UseNullPropagation
            if (value is DateTime) return ((DateTime)value).UtcToLocalFromSettings();

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