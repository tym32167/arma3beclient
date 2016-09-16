using System;
using Arma3BEClient.Libs.ModelCompact;
using Arma3BEClient.Libs.Tools;

namespace Arma3BE.Client.Infrastructure.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime UtcToLocalFromSettings(this DateTime source)
        {
            var zone = SettingsStore.Instance.TimeZoneInfo;
            return TimeZoneInfo.ConvertTimeFromUtc(source, zone);
        }


        public static DateTime LocalToUtcFromSettings(this DateTime source)
        {
            var zone = SettingsStore.Instance.TimeZoneInfo;
            return TimeZoneInfo.ConvertTimeToUtc(source, zone);
        }


        public static DateTime? UtcToLocalFromSettings(this DateTime? source)
        {
            return source?.UtcToLocalFromSettings();
        }


        public static DateTime? LocalToUtcFromSettings(this DateTime? source)
        {
            return source?.LocalToUtcFromSettings();
        }
    }
}