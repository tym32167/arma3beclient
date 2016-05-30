using Arma3BE.Server.Abstract;
using Arma3BEClient.Common.Logging;
using BattleNET;

namespace Arma3BE.Server.Mocks
{
    public class BattlEyeServerLogProxy : IBattlEyeServer
    {
        private readonly IBattlEyeServer _server;

        public BattlEyeServerLogProxy(IBattlEyeServer server, ILog log)
        {
            _server = server;
        }

        public void Dispose()
        {
            _server?.Dispose();
        }

        public bool Connected { get; }

        public int SendCommand(BattlEyeCommand command, string parameters = "")
        {
            return _server?.SendCommand(command, parameters) ?? 0;
        }

        public void Disconnect()
        {
           _server?.Disconnect();
        }

        public event BattlEyeMessageEventHandler BattlEyeMessageReceived;
        public event BattlEyeConnectEventHandler BattlEyeConnected;
        public event BattlEyeDisconnectEventHandler BattlEyeDisconnected;


        public BattlEyeConnectionResult Connect()
        {
            return _server?.Connect() ?? BattlEyeConnectionResult.ConnectionFailed;
        }
    }
}