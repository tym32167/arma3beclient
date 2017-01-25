using System;

namespace Arma3BEClient.Libs.Tools
{
    public interface ISettingsStore : ICloneable
    {
        string AdminName { get; set; }

        string BanMessageTemplate { get; set; }
        string KickMessageTemplate { get; set; }

        int BansUpdateSeconds { get; set; }
        int PlayersUpdateSeconds { get; set; }
        TimeZoneInfo TimeZoneInfo { get; set; }

        void Save();


        void Save(string key, string value);
        string Load(string key);
    }

    public interface ISettingsStoreSource
    {
        ISettingsStore GetSettingsStore();
    }

    public class SettingsStoreSource : ISettingsStoreSource
    {
        public ISettingsStore GetSettingsStore()
        {
            return SettingsStore.Instance;
        }
    }
}