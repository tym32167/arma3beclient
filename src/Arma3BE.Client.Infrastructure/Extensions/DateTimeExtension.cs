using Arma3BEClient.Libs.Tools;
using System;

namespace Arma3BE.Client.Infrastructure.Extensions
{
    public static class DateTimeExtensions
    {
        private static ISettingsStore SettingsStore => new SettingsStoreSource().GetSettingsStore();

        public static DateTime UtcToLocalFromSettings(this DateTime source)
        {
            var zone = SettingsStore.TimeZoneInfo;
            return TimeZoneInfo.ConvertTimeFromUtc(source, zone);
        }


        private static DateTime LocalToUtcFromSettings(this DateTime source)
        {
            var zone = SettingsStore.TimeZoneInfo;
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