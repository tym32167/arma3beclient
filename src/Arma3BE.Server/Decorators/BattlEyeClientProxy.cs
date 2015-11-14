using Arma3BEClient.Common.Core;
using BattleNET;

namespace Arma3BE.Server.Decorators
{
    public class BattlEyeClientProxy : DisposeObject, IBattlEyeClient
    {
        private BattlEyeClient _battlEyeClient;

        public BattlEyeClientProxy(BattlEyeClient client)
        {
            _battlEyeClient = client;

            _battlEyeClient.BattlEyeConnected += OnBattlEyeConnected;
            _battlEyeClient.BattlEyeMessageReceived += OnBattlEyeMessageReceived;
            _battlEyeClient.BattlEyeDisconnected += OnBattlEyeDisconnected;
        }

        public event BattlEyeMessageEventHandler BattlEyeMessageReceived;
        public event BattlEyeConnectEventHandler BattlEyeConnected;
        public event BattlEyeDisconnectEventHandler BattlEyeDisconnected;

        public bool Connected => _battlEyeClient.Connected;


        private void OnBattlEyeMessageReceived(BattlEyeMessageEventArgs message)
        {
            if (message != null && !string.IsNullOrEmpty(message.Message))
                BattlEyeMessageReceived?.Invoke(message);
        }

        private void OnBattlEyeConnected(BattlEyeConnectEventArgs args)
        {
            if (Connected)
                BattlEyeConnected?.Invoke(args);
        }

        private void OnBattlEyeDisconnected(BattlEyeDisconnectEventArgs args)
        {
            BattlEyeDisconnected?.Invoke(args);
        }

        public BattlEyeConnectionResult Connect()
        {
            return _battlEyeClient.Connect();
        }

        public void Disconnect()
        {
            _battlEyeClient?.Disconnect();
        }

        public bool ReconnectOnPacketLoss
        {
            get { return _battlEyeClient.ReconnectOnPacketLoss; }
            set { _battlEyeClient.ReconnectOnPacketLoss = value; }
        }

        public int SendCommand(BattlEyeCommand command, string parameters = "")
        {
            return _battlEyeClient.SendCommand(command, parameters);
        }

        private object _lock = new object();

        protected override void DisposeManagedResources()
        {
            if (_battlEyeClient != null)
            {
                lock (_lock)
                {
                    if (_battlEyeClient != null)
                    {
                        _battlEyeClient.BattlEyeConnected -= OnBattlEyeConnected;
                        _battlEyeClient.BattlEyeMessageReceived -= OnBattlEyeMessageReceived;
                        _battlEyeClient.BattlEyeDisconnected -= OnBattlEyeDisconnected;

                        if (_battlEyeClient.Connected) _battlEyeClient.Disconnect();
                        _battlEyeClient = null;
                    }
                }
            }
            base.DisposeManagedResources();
        }
    }
}