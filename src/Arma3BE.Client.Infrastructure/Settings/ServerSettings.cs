using System;

namespace Arma3BE.Client.Infrastructure.Settings
{
    public class ServerSettings
    {
        public string IdleKickReason { get; set; }
        public int IdleTimeInMins { get; set; }
        public bool KickIdlePlayers { get; set; }

        public Guid ServerId { get; set; }
    }
}