using Arma3BE.Server.Abstract;
using Arma3BE.Server.ServerDecorators;
using BattleNET;

namespace Arma3BE.Server.ServerFactory
{
    public class WatcherBEServerFactory : IBattlEyeServerFactory
    {
        public IBattlEyeServer Create(BattlEyeLoginCredentials credentials)
        {
            return new BEConnectedWatcher(new BattlEyeServerFactory(), credentials);
        }
    }
}