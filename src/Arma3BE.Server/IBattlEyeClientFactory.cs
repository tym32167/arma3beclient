using Arma3BEClient.Common.Logging;
using BattleNET;

namespace Arma3BE.Server
{
    public interface IBattlEyeClientFactory
    {
        IBattlEyeClient Create(BattlEyeLoginCredentials credentials);
    }

    public class BattlEyeClientFactory : IBattlEyeClientFactory
    {
        private readonly ILog _log;

        public BattlEyeClientFactory(ILog log)
        {
            _log = log;
        }

        public IBattlEyeClient Create(BattlEyeLoginCredentials credentials)
        {
            return new ThreadSafeBattleEyeClient(new BattlEyeClient(credentials), _log);
        }
    }
}