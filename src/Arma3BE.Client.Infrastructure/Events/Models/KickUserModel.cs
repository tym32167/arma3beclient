using Arma3BE.Server.Abstract;

namespace Arma3BE.Client.Infrastructure.Events.Models
{
    public class KickUserModel
    {
        public string PlayerGuid { get; }
        public int PlayerNum { get; }
        public IBEServer BEServer { get; }
        public string PlayerName { get; }

        public KickUserModel(IBEServer beServer, string playerGuid, string playerName,
            int playerNum)
        {
            PlayerGuid = playerGuid;
            PlayerNum = playerNum;
            BEServer = beServer;
            PlayerName = playerName;
        }
    }
}