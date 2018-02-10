using System;

namespace Arma3BEClient.Libs.Core.Settings
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
}