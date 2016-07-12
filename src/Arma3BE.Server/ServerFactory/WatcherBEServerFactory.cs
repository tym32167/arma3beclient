using Arma3BE.Server.Abstract;
using Arma3BE.Server.ServerDecorators;
using Arma3BEClient.Common.Logging;
using BattleNET;

namespace Arma3BE.Server.ServerFactory
{
    public class WatcherBEServerFactory : IBattlEyeServerFactory
    {
        private readonly ILog _log;

        public WatcherBEServerFactory(ILog log)
        {
            _log = log;
        }

        public IBattlEyeServer Create(BattlEyeLoginCredentials credentials)
        {
            return new BEConnectedWatcher(new BattlEyeServerFactory(_log), _log, credentials);
        }
    }
}