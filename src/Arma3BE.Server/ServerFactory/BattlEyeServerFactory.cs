using Arma3BE.Server.Abstract;
using Arma3BE.Server.Mocks;
using Arma3BE.Server.ServerDecorators;
using Arma3BEClient.Common.Logging;
using BattleNET;

namespace Arma3BE.Server.ServerFactory
{
    public class BattlEyeServerFactory : IBattlEyeServerFactory
    {
        private readonly ILog _log;

        public BattlEyeServerFactory(ILog log)
        {
            _log = log;
        }

        public IBattlEyeServer Create(BattlEyeLoginCredentials credentials)
        {
            return new ThreadSafeBattleEyeServer(new BattlEyeServerProxy(new BattlEyeClient(credentials)
            {
                ReconnectOnPacketLoss = true
            }), _log);

            //return new BattlEyeServerLogProxy(new MockBattleEyeServer(), _log);
        }
    }
}