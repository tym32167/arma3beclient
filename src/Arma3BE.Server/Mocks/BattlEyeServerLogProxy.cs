using Arma3BE.Server.Abstract;
using Arma3BEClient.Common.Logging;
using BattleNET;

namespace Arma3BE.Server.Mocks
{
    public class BattlEyeServerLogProxy : IBattlEyeServer
    {
        private IBattlEyeServer _server;
        private readonly ILog _log;

        public BattlEyeServerLogProxy(IBattlEyeServer server, ILog log)
        {
            _server = server;
            _log = log;

            _server.BattlEyeConnected += OnBattlEyeConnected;
            _server.BattlEyeDisconnected += OnBattlEyeDisconnected;
            _server.BattlEyeMessageReceived += OnBattlEyeMessageReceived;
        }

        public void Dispose()
        {
            var local = _server;
            if (local != null)
            {
                local.BattlEyeConnected += OnBattlEyeConnected;
                local.BattlEyeDisconnected += OnBattlEyeDisconnected;
                local.BattlEyeMessageReceived += OnBattlEyeMessageReceived;
                local.Dispose();
            }
            _server = null;
        }

        public bool Connected => _server?.Connected ?? false;

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

        protected virtual void OnBattlEyeMessageReceived(BattlEyeMessageEventArgs args)
        {
            _log.Info(args.Message);
            BattlEyeMessageReceived?.Invoke(args);
        }

        protected virtual void OnBattlEyeConnected(BattlEyeConnectEventArgs args)
        {
            BattlEyeConnected?.Invoke(args);
        }

        protected virtual void OnBattlEyeDisconnected(BattlEyeDisconnectEventArgs args)
        {
            BattlEyeDisconnected?.Invoke(args);
        }
    }
}