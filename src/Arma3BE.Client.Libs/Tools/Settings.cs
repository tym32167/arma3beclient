using System;
using System.Data.Entity.Migrations;
using System.Linq;
using Arma3BEClient.Libs.Context;
using Arma3BEClient.Libs.ModelCompact;

namespace Arma3BEClient.Libs.Tools
{
    public class SettingsStore
    {
        private const int AdminNameKey = 1;
        private const int TimeZoneKey = 2;

        public string AdminName { get; set; }
        public TimeZoneInfo TimeZoneInfo { get; set; }

        private static SettingsStore _instance;
        public static SettingsStore Instance => _instance ?? (_instance = Load());

        public void Save()
        {
            using (var context = new Arma3BeClientContext())
            {
                context.Settings.AddOrUpdate(
                    new Settings {Id = AdminNameKey, Value = AdminName}, 
                    new Settings {Id = TimeZoneKey, Value = TimeZoneInfo?.Id});

                context.SaveChanges();
                _instance = null;
            }
        }

        private static SettingsStore Load()
        {
            using (var context = new Arma3BeClientContext())
            {
                var settings = context.Settings.ToArray();

                var ss = new SettingsStore();
                ss.AdminName = settings.FirstOrDefault(x => x.Id == AdminNameKey)?.Value ?? "Admin";
                
                try
                {
                    var zone = settings.FirstOrDefault(x => x.Id == TimeZoneKey)?.Value;
                    ss.TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(zone);
                }
                catch (Exception)
                {
                    ss.TimeZoneInfo = TimeZoneInfo.Local;
                }

                return ss;
            }
        }
    }
}