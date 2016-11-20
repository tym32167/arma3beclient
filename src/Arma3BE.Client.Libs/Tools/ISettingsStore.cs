using System;

namespace Arma3BEClient.Libs.Tools
{
    public interface ISettingsStore : ICloneable
    {
        string AdminName { get; set; }
        int BansUpdateSeconds { get; set; }
        int PlayersUpdateSeconds { get; set; }
        TimeZoneInfo TimeZoneInfo { get; set; }

        void Save();
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