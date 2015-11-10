using Arma3BEClient.Common.Logging;
using BattleNET;

namespace Arma3BE.Server
{
    public class BEConnectedWatcher : IBattlEyeClient
    {
        private IBattlEyeClient _battlEyeClient;
        private readonly IBattlEyeClientFactory _battlEyeClientFactory;
        private readonly ILog _log;
        private readonly BattlEyeLoginCredentials _credentials;

        private object _lock = new object();


        public BEConnectedWatcher(IBattlEyeClientFactory battlEyeClientFactory, ILog log, BattlEyeLoginCredentials credentials)
        {
            _battlEyeClientFactory = battlEyeClientFactory;
            _log = log;
            _credentials = credentials;

            Init();
        }

        private void Init()
        {
            lock (_lock)
            {
                Release();

                _battlEyeClient = _battlEyeClientFactory.Create(_credentials);
                _battlEyeClient.BattlEyeConnected += OnBattlEyeConnected;
                _battlEyeClient.BattlEyeMessageReceived += OnBattlEyeMessageReceived;
                _battlEyeClient.BattlEyeDisconnected += OnBattlEyeDisconnected;
            }
        }

        private void Release()
        {
            lock (_lock)
            {
                if (_battlEyeClient != null)
                {
                    _battlEyeClient.BattlEyeConnected -= OnBattlEyeConnected;
                    _battlEyeClient.BattlEyeMessageReceived -= OnBattlEyeMessageReceived;
                    _battlEyeClient.BattlEyeDisconnected -= OnBattlEyeDisconnected;

                    if (_battlEyeClient.Connected) _battlEyeClient.Disconnect();
                }

                _battlEyeClient = null;
            }
        }

        public bool Connected => _battlEyeClient.Connected;


        public bool ReconnectOnPacketLoss
        {
            get { return _battlEyeClient.ReconnectOnPacketLoss; }
            set { _battlEyeClient.ReconnectOnPacketLoss = value; }
        }

        public int SendCommand(BattlEyeCommand command, string parameters = "")
        {
            _battlEyeClient?.SendCommand(command, parameters);
            return 0;
        }

        public BattlEyeConnectionResult Connect()
        {
            var result = _battlEyeClient?.Connect();
            return result ?? BattlEyeConnectionResult.ConnectionFailed;
        }

        public void Disconnect()
        {
            _battlEyeClient?.Disconnect();
        }

        public event BattlEyeMessageEventHandler BattlEyeMessageReceived;
        public event BattlEyeConnectEventHandler BattlEyeConnected;
        public event BattlEyeDisconnectEventHandler BattlEyeDisconnected;


        private void OnBattlEyeMessageReceived(BattlEyeMessageEventArgs message)
        {
            BattlEyeMessageReceived?.Invoke(message);
        }

        private void OnBattlEyeConnected(BattlEyeConnectEventArgs args)
        {
            BattlEyeConnected?.Invoke(args);
        }

        private void OnBattlEyeDisconnected(BattlEyeDisconnectEventArgs args)
        {
            BattlEyeDisconnected?.Invoke(args);
        }

    }
}