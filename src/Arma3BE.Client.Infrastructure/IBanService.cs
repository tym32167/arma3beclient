using Arma3BE.Server.Abstract;

namespace Arma3BE.Client.Infrastructure
{
    public interface IBanService
    {
        void ShowBanDialog(IBEServer beServer, string playerGuid, bool isOnline, string playerName,
            string playerNum);

        void ShowKickDialog(IBEServer beServer, int playerNum, string playerGuid, string playerName);
    }
}