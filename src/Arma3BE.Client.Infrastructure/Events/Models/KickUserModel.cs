using System;

namespace Arma3BE.Client.Infrastructure.Events.Models
{
    public class KickUserModel
    {
        public Guid ServerId { get; }
        public string PlayerGuid { get; }
        public int PlayerNum { get; }
        public string PlayerName { get; }

        public KickUserModel(Guid serverId, string playerGuid, string playerName,
            int playerNum)
        {
            ServerId = serverId;
            PlayerGuid = playerGuid;
            PlayerNum = playerNum;
            PlayerName = playerName;
        }
    }
}