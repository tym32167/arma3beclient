using System;

namespace Arma3BE.Client.Infrastructure.Events.Models
{
    public class BanUserModel
    {
        public Guid ServerId { get; }
        public string PlayerGuid { get; }
        public string PlayerNum { get; }
        public bool IsOnline { get; }
        public string PlayerName { get; }

        public BanUserModel(Guid serverId, string playerGuid, bool isOnline, string playerName,
            string playerNum)
        {
            ServerId = serverId;
            PlayerGuid = playerGuid;
            PlayerNum = playerNum;
            IsOnline = isOnline;
            PlayerName = playerName;
        }
    }
}