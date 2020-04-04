using System;

namespace Arma3BE.Client.Modules.SyncModule.SyncCore
{
    public class PlayerSyncDto
    {
        public string SteamId { get; set; }
        public string GUID { get; set; }
        public string Comment { get; set; }
        public string Name { get; set; }
        public string IP { get; set; }
        public DateTime LastSeen { get; set; }
    }

    public class PlayerSyncResponse
    {
        public int Count { get; set; }
        public PlayerSyncDto[] Players { get; set; }
    }
}