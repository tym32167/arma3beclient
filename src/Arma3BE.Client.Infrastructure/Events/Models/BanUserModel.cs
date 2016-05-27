using Arma3BE.Server.Abstract;

namespace Arma3BE.Client.Infrastructure.Events.Models
{
    public class BanUserModel
    {
        public string PlayerGuid { get; }
        public string PlayerNum { get; }
        public IBEServer BEServer { get; }
        public bool IsOnline { get; }
        public string PlayerName { get; }

        public BanUserModel(IBEServer beServer, string playerGuid, bool isOnline, string playerName,
            string playerNum)
        {
            PlayerGuid = playerGuid;
            PlayerNum = playerNum;
            BEServer = beServer;
            IsOnline = isOnline;
            PlayerName = playerName;
        }
    }
}