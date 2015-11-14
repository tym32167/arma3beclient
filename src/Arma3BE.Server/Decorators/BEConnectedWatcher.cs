using System.Timers;
using Arma3BEClient.Common.Core;
using Arma3BEClient.Common.Logging;
using BattleNET;

namespace Arma3BE.Server.Decorators
{
    public class BEConnectedWatcher :DisposeObject, IBattlEyeClient
    {
        private IBattlEyeClient _battlEyeClient;
        private readonly IBattlEyeClientFactory _battlEyeClientFactory;
        private readonly ILog _log;
        private readonly BattlEyeLoginCredentials _credentials;

        private readonly System.Timers.Timer _timer;

        private readonly object _lock = new object();


        public BEConnectedWatcher(IBattlEyeClientFactory battlEyeClientFactory, ILog log, BattlEyeLoginCredentials credentials)
        {
            _battlEyeClientFactory = battlEyeClientFactory;
            _log = log;
            _credentials = credentials;

            Init();


            _timer = new Timer(5000);
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
        }


        private int _numAttempts = 0;
        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_battlEyeClient == null || !_battlEyeClient.Connected)
            {
                _numAttempts++;
            }
            else
            {
                _numAttempts = 0;
            }

            _log.Info($"ATTEMPTS {_numAttempts} FOR {_credentials.Host}:{_credentials.Port}");
            if (_numAttempts > 5)
            {
                _numAttempts = 0;
                _log.Info($"RECREATE CLIENT FOR {_credentials.Host}:{_credentials.Port}");
                Release();
                Init();
            }
        }

        private void Init()
        {
            lock (_lock)
            {
                _log.Info($"Init {_credentials.Host}:{_credentials.Port}");

                _battlEyeClient = _battlEyeClientFactory.Create(_credentials);
                _battlEyeClient.BattlEyeConnected += OnBattlEyeConnected;
                _battlEyeClient.BattlEyeMessageReceived += OnBattlEyeMessageReceived;
                _battlEyeClient.BattlEyeDisconnected += OnBattlEyeDisconnected;

                _battlEyeClient.Connect();
            }
        }

        private void Release()
        {
            lock (_lock)
            {
                _log.Info($"Release {_credentials.Host}:{_credentials.Port}");
                if (_battlEyeClient != null)
                {
                    _battlEyeClient.BattlEyeConnected -= OnBattlEyeConnected;
                    _battlEyeClient.BattlEyeMessageReceived -= OnBattlEyeMessageReceived;
                    _battlEyeClient.BattlEyeDisconnected -= OnBattlEyeDisconnected;

                    if (_battlEyeClient.Connected) _battlEyeClient.Disconnect();

                    _battlEyeClient.Dispose();
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

        protected override void DisposeManagedResources()
        {
            if (_battlEyeClient != null)
            {
                lock (_lock)
                {
                    _timer.Stop();
                    if (_battlEyeClient != null)
                    {
                        Release();
                    }
                }
            }

            
            base.DisposeManagedResources();
        }
    }
}