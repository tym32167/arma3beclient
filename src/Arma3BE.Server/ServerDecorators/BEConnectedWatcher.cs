using Arma3BE.Server.Abstract;
using Arma3BEClient.Common.Core;
using Arma3BEClient.Common.Logging;
using BattleNET;
using System;
using System.Diagnostics;
using System.Threading;

namespace Arma3BE.Server.ServerDecorators
{
    public class BEConnectedWatcher : DisposeObject, IBattlEyeServer
    {
        private readonly IBattlEyeServerFactory _battlEyeServerFactory;
        private readonly BattlEyeLoginCredentials _credentials;

        private readonly object _lock = new object();
        private readonly ILog _log = LogFactory.Create(new StackTrace().GetFrame(0).GetMethod().DeclaringType);

        private readonly Timer _timer;
        private IBattlEyeServer _battlEyeServer;
        private DateTime _lastReceived = DateTime.UtcNow;
        private int _numAttempts;

        public bool Connected => _battlEyeServer != null && _battlEyeServer.Connected;

        public BEConnectedWatcher(IBattlEyeServerFactory battlEyeServerFactory,
            BattlEyeLoginCredentials credentials)
        {
            _battlEyeServerFactory = battlEyeServerFactory;
            _credentials = credentials;

            _timer = new Timer(_timer_Elapsed, null, 5000, 10000);
            Init();
        }

        public int SendCommand(BattlEyeCommand command, string parameters = "")
        {
            _battlEyeServer?.SendCommand(command, parameters);
            return 0;
        }

        public BattlEyeConnectionResult Connect()
        {
            var result = _battlEyeServer?.Connect();
            return result ?? BattlEyeConnectionResult.ConnectionFailed;
        }

        public void Disconnect()
        {
            _battlEyeServer?.Disconnect();
        }

        public event BattlEyeMessageEventHandler BattlEyeMessageReceived;
        public event BattlEyeConnectEventHandler BattlEyeConnected;
        public event BattlEyeDisconnectEventHandler BattlEyeDisconnected;

        private void _timer_Elapsed(object state)
        {
            if (_battlEyeServer == null || !_battlEyeServer.Connected)
            {
                _numAttempts++;
            }
            else
            {
                _numAttempts = 0;
            }

            var lastReceivedSpan = DateTime.UtcNow - _lastReceived;

            //_log.Info($"ATTEMPTS {_numAttempts} FOR {_credentials.Host}:{_credentials.Port} WITH LAST RECEIVED {lastReceivedSpan}");
            if (_numAttempts > 5 || lastReceivedSpan.TotalMinutes > 15)
            {
                _numAttempts = 0;
                _lastReceived = DateTime.UtcNow;
                _log.Info($"RECREATE CLIENT FOR {_credentials.Host}:{_credentials.Port} WITH LAST RECEIVED {lastReceivedSpan}");

                Release();

                OnBattlEyeDisconnected(new BattlEyeDisconnectEventArgs(_credentials,
                    BattlEyeDisconnectionType.ConnectionLost));

                Init();
                Connect();
            }
        }

        private void Init()
        {
            lock (_lock)
            {
                _log.Info($"Init {_credentials.Host}:{_credentials.Port}");

                _battlEyeServer = _battlEyeServerFactory.Create(_credentials);
                _battlEyeServer.BattlEyeConnected += OnBattlEyeConnected;
                _battlEyeServer.BattlEyeMessageReceived += OnBattlEyeMessageReceived;
                _battlEyeServer.BattlEyeDisconnected += OnBattlEyeDisconnected;
            }
        }

        private void Release()
        {
            lock (_lock)
            {
                _log.Info($"Release {_credentials.Host}:{_credentials.Port}");
                if (_battlEyeServer != null)
                {
                    _battlEyeServer.BattlEyeConnected -= OnBattlEyeConnected;
                    _battlEyeServer.BattlEyeMessageReceived -= OnBattlEyeMessageReceived;
                    _battlEyeServer.BattlEyeDisconnected -= OnBattlEyeDisconnected;

                    if (_battlEyeServer.Connected) _battlEyeServer.Disconnect();

                    _battlEyeServer.Dispose();
                }

                _battlEyeServer = null;
            }
        }


        private void OnBattlEyeMessageReceived(BattlEyeMessageEventArgs message)
        {
            _lastReceived = DateTime.UtcNow;
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
            _timer.Dispose();

            if (_battlEyeServer != null)
            {
                lock (_lock)
                {
                    if (_battlEyeServer != null)
                    {
                        Release();
                    }
                }
            }

            base.DisposeManagedResources();
        }
    }
}