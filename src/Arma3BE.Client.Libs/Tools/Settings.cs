﻿using Arma3BEClient.Common.Extensions;
using Arma3BEClient.Libs.Context;
using Arma3BEClient.Libs.ModelCompact;
using System;
using System.Collections.Concurrent;
using System.Data.Entity.Migrations;
using System.Linq;

namespace Arma3BEClient.Libs.Tools
{
    internal class SettingsStore : ISettingsStore
    {
        private const int AdminNameKey = 1;
        private const int TimeZoneKey = 2;
        private const int PlayersUpdateKey = 3;
        private const int BansUpdateKey = 4;

        private const int BanMessageTemplateKey = 5;
        private const int KickMessageTemplateKey = 6;

        private const int TopMostKey = 7;

        private const int SteamFolderKey = 8;

        public string AdminName { get; set; }

        public string BanMessageTemplate { get; set; }
        public string KickMessageTemplate { get; set; }

        public TimeZoneInfo TimeZoneInfo { get; set; }
        public int PlayersUpdateSeconds { get; set; }
        public int BansUpdateSeconds { get; set; }

        public bool TopMost { get; set; }

        public string SteamFolder { get; set; }


        private const int IdleTimeInMinsKey = 9;
        public int IdleTimeInMins { get; set; }

        private const int IdleKickTextKey = 10;
        public string IdleKickText { get; set; }



        private static SettingsStore _instance;
        public static SettingsStore Instance => _instance ?? (_instance = Load());

        public void Save()
        {
            using (var context = new Arma3BeClientContext())
            {
                context.Settings.AddOrUpdate(
                    new Settings { Id = AdminNameKey, Value = AdminName },
                    new Settings { Id = TimeZoneKey, Value = TimeZoneInfo?.Id },
                    new Settings { Id = PlayersUpdateKey, Value = PlayersUpdateSeconds.ToString() },
                    new Settings { Id = BansUpdateKey, Value = BansUpdateSeconds.ToString() },
                    new Settings { Id = BanMessageTemplateKey, Value = BanMessageTemplate },
                    new Settings { Id = KickMessageTemplateKey, Value = KickMessageTemplate },
                    new Settings { Id = TopMostKey, Value = TopMost.ToString() },
                    new Settings { Id = SteamFolderKey, Value = SteamFolder },

                    new Settings { Id = IdleTimeInMinsKey, Value = IdleTimeInMins.ToString() },
                    new Settings { Id = IdleKickTextKey, Value = IdleKickText }
                );

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


                ss.BanMessageTemplate = settings.FirstOrDefault(x => x.Id == BanMessageTemplateKey)?.Value ??
                                        "[{AdminName}][{Date} {Time}] {Reason} {Minutes}";
                ss.KickMessageTemplate = settings.FirstOrDefault(x => x.Id == KickMessageTemplateKey)?.Value ??
                                         "[{AdminName}][{Date} {Time}] {Reason}";


                ss.PlayersUpdateSeconds = (settings.FirstOrDefault(x => x.Id == PlayersUpdateKey)?.Value).FromString(5);
                ss.BansUpdateSeconds = (settings.FirstOrDefault(x => x.Id == BansUpdateKey)?.Value).FromString(5);

                try
                {
                    var zone = settings.FirstOrDefault(x => x.Id == TimeZoneKey)?.Value;
                    ss.TimeZoneInfo = zone != null ? TimeZoneInfo.FindSystemTimeZoneById(zone) : TimeZoneInfo.Local;
                }
                catch (Exception)
                {
                    ss.TimeZoneInfo = TimeZoneInfo.Local;
                }

                ss.TopMost = bool.Parse(settings.FirstOrDefault(x => x.Id == TopMostKey)?.Value ?? bool.FalseString);

                ss.SteamFolder = settings.FirstOrDefault(x => x.Id == SteamFolderKey)?.Value;

                ss.IdleKickText = settings.FirstOrDefault(x => x.Id == IdleKickTextKey)?.Value ?? "Lobby Idle";
                ss.IdleTimeInMins = (settings.FirstOrDefault(x => x.Id == IdleTimeInMinsKey)?.Value).FromString(0);

                return ss;
            }
        }

        public object Clone()
        {
            return new SettingsStore
            {
                TimeZoneInfo = TimeZoneInfo,
                AdminName = AdminName,
                BansUpdateSeconds = BansUpdateSeconds,
                PlayersUpdateSeconds = PlayersUpdateSeconds,
                BanMessageTemplate = BanMessageTemplate,
                KickMessageTemplate = KickMessageTemplate,
                TopMost = TopMost,
                SteamFolder = SteamFolder,
                IdleTimeInMins = IdleTimeInMins,
                IdleKickText = IdleKickText
            };
        }
    }

    public class CustomSettingsStore : ICustomSettingsStore
    {
        readonly ConcurrentDictionary<string, string> _cache = new ConcurrentDictionary<string, string>();


        private static readonly Lazy<CustomSettingsStore> _instance =
            new Lazy<CustomSettingsStore>(() => new CustomSettingsStore());

        public static CustomSettingsStore Instance => _instance.Value;

        public void Save(string key, string value)
        {
            _cache.AddOrUpdate(key, k => value, (k, v) => value);
            SaveInternal(key, value);
        }

        public string Load(string key)
        {
            var rrr = _cache.GetOrAdd(key, LoadInternal);
            return rrr;
        }

        private string LoadInternal(string key)
        {
            using (var context = new Arma3BeClientContext())
            {
                return context.CustomSettings.FirstOrDefault(x => x.Id == key)?.Value;
            }
        }

        private void SaveInternal(string key, string value)
        {
            using (var context = new Arma3BeClientContext())
            {
                context.CustomSettings.AddOrUpdate(new CustomSettings { Id = key, Value = value });
                context.SaveChanges();
            }
        }
    }
}