using Arma3BE.Server.Abstract;
using Arma3BE.Server.ServerDecorators;
using BattleNET;

namespace Arma3BE.Server.ServerFactory
{
    public class BattlEyeServerFactory : IBattlEyeServerFactory
    {
        public IBattlEyeServer Create(BattlEyeLoginCredentials credentials)
        {
            return new ThreadSafeBattleEyeServer(new BattlEyeServerProxy(new BattlEyeClient(credentials)
            {
                ReconnectOnPacketLoss = true
            }, $"{credentials.Host}:{credentials.Port}"));
        }
    }
}