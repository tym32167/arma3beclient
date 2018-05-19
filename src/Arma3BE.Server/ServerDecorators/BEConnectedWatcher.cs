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

        private static readonly ILog Log = LogFactory.Create(new StackTrace().GetFrame(0).GetMethod().DeclaringType);

        private readonly Timer _timer;
        private readonly Timer _keepAliveTimer;

        private volatile IBattlEyeServer _battlEyeServer;
        private DateTime _lastReceived = DateTime.UtcNow;
        private int _numAttempts;

        public bool Connected => _battlEyeServer != null && _battlEyeServer.Connected;

        public BEConnectedWatcher(IBattlEyeServerFactory battlEyeServerFactory,
            BattlEyeLoginCredentials credentials)
        {
            _battlEyeServerFactory = battlEyeServerFactory;
            _credentials = credentials;

            _timer = new Timer(_timer_Elapsed, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10));
            _keepAliveTimer = new Timer(_timer_KeepAlive, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));

            _battlEyeServer = Init(_credentials);
        }

        public int SendCommand(BattlEyeCommand command, string parameters = "")
        {
            _battlEyeServer?.SendCommand(command, parameters);
            return 0;
        }

        public int SendCommand(string command)
        {
            _battlEyeServer?.SendCommand(command);
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

        private void _timer_KeepAlive(object state)
        {
            var local = _battlEyeServer;
            if (local?.Connected == true)
            {
                local.SendCommand(BattlEyeCommand.Players);
            }
        }

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
                Log.Info(
                    $"RECREATE CLIENT FOR {_credentials.Host}:{_credentials.Port} WITH LAST RECEIVED {lastReceivedSpan}");

                var local = _battlEyeServer;
                _battlEyeServer = null;
                if (local != null) Release(local, _credentials);

                OnBattlEyeDisconnected(new BattlEyeDisconnectEventArgs(_credentials,
                    BattlEyeDisconnectionType.ConnectionLost));

                Init(_credentials);

                _battlEyeServer = Init(_credentials);

                Connect();
            }
        }

        private IBattlEyeServer Init(BattlEyeLoginCredentials battlEyeLoginCredentials)
        {
            Log.Info($"Init {battlEyeLoginCredentials.Host}:{battlEyeLoginCredentials.Port}");

            var battlEyeServer = _battlEyeServerFactory.Create(battlEyeLoginCredentials);
            battlEyeServer.BattlEyeConnected += OnBattlEyeConnected;
            battlEyeServer.BattlEyeMessageReceived += OnBattlEyeMessageReceived;
            battlEyeServer.BattlEyeDisconnected += OnBattlEyeDisconnected;

            return battlEyeServer;
        }

        private void Release(IBattlEyeServer battlEyeServer, BattlEyeLoginCredentials battlEyeLoginCredentials)
        {
            Log.Info($"Release {battlEyeLoginCredentials.Host}:{battlEyeLoginCredentials.Port}");
            if (battlEyeServer != null)
            {
                battlEyeServer.BattlEyeConnected -= OnBattlEyeConnected;
                battlEyeServer.BattlEyeMessageReceived -= OnBattlEyeMessageReceived;
                battlEyeServer.BattlEyeDisconnected -= OnBattlEyeDisconnected;

                if (battlEyeServer.Connected) battlEyeServer.Disconnect();

                battlEyeServer.Dispose();
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
            _timer?.Dispose();
            _keepAliveTimer?.Dispose();

            var local = _battlEyeServer;
            _battlEyeServer = null;
            if (local != null) Release(local, _credentials);


            base.DisposeManagedResources();
        }
    }
}