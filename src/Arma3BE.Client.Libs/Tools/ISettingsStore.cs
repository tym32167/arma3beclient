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

        bool TopMost { get; set; }

        string SteamFolder { get; set; }

        void Save();
    }


    public interface ICustomSettingsStore
    {
        void Save(string key, string value);
        string Load(string key);
    }

    public interface ISettingsStoreSource
    {
        ISettingsStore GetSettingsStore();
        ICustomSettingsStore GetCustomSettingsStore();
    }

    public class SettingsStoreSource : ISettingsStoreSource
    {
        public ISettingsStore GetSettingsStore()
        {
            return SettingsStore.Instance;
        }

        public ICustomSettingsStore GetCustomSettingsStore()
        {
            return CustomSettingsStore.Instance;
        }
    }
}