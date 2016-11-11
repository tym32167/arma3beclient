using Arma3BE.Server.Abstract;
using Arma3BE.Server.Mocks;
using Arma3BE.Server.ServerDecorators;
using Arma3BEClient.Common.Logging;
using BattleNET;
using System.Configuration;

namespace Arma3BE.Server.ServerFactory
{
    public class BattlEyeServerFactory : IBattlEyeServerFactory
    {
        private readonly ILog _log;
        private const string DebugServerKey = "DebugServerEnabled";

        public BattlEyeServerFactory(ILog log)
        {
            _log = log;
        }

        public IBattlEyeServer Create(BattlEyeLoginCredentials credentials)
        {

            if (ConfigurationManager.AppSettings[DebugServerKey] == bool.TrueString)
            {
                return new BattlEyeServerLogProxy(new MockBattleEyeServer(), _log);
            }

            return new ThreadSafeBattleEyeServer(new BattlEyeServerProxy(new BattlEyeClient(credentials)
            {
                ReconnectOnPacketLoss = true
            }, $"{credentials.Host}:{credentials.Port}"), _log);
        }
    }
}