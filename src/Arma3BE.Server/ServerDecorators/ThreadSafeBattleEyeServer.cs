using Arma3BE.Server.Abstract;
using Arma3BEClient.Common.Core;
using Arma3BEClient.Common.Logging;
using BattleNET;
using System.Diagnostics;

// ReSharper disable MemberCanBePrivate.Local

namespace Arma3BE.Server.ServerDecorators
{
    public class ThreadSafeBattleEyeServer : DisposeObject, IBattlEyeServer
    {
        private readonly object _lock = new object();
        private static readonly ILog Log = LogFactory.Create(new StackTrace().GetFrame(0).GetMethod().DeclaringType);
        private volatile IBattlEyeServer _battlEyeServer;

        public ThreadSafeBattleEyeServer(IBattlEyeServer battlEyeServer)
        {
            _battlEyeServer = battlEyeServer;
            _battlEyeServer.BattlEyeConnected += OnBattlEyeConnected;
            _battlEyeServer.BattlEyeMessageReceived += OnBattlEyeMessageReceived;
            _battlEyeServer.BattlEyeDisconnected += OnBattlEyeDisconnected;

            Log.Info($"ThreadSafeBattleEyeClient Init");
        }

        public bool Connected => _battlEyeServer?.Connected == true;

        public int SendCommand(BattlEyeCommand command, string parameters = "")
        {
            lock (_lock)
            {
                return _battlEyeServer?.SendCommand(command, parameters) ?? 0;
            }
        }

        public int SendCommand(string command)
        {
            lock (_lock)
            {
                return _battlEyeServer?.SendCommand(command) ?? 0;
            }
        }

        public void Disconnect()
        {
            lock (_lock)
            {
                Log.Info($"TRY TO DISCONNECT FROM");
                _battlEyeServer?.Disconnect();
            }
        }

        public BattlEyeConnectionResult Connect()
        {
            lock (_lock)
            {
                Log.Info($"TRY TO CONNECT TO");
                return _battlEyeServer?.Connect() ?? 0;
            }
        }

        public event BattlEyeMessageEventHandler BattlEyeMessageReceived;
        public event BattlEyeConnectEventHandler BattlEyeConnected;
        public event BattlEyeDisconnectEventHandler BattlEyeDisconnected;


        private void OnBattlEyeMessageReceived(BattlEyeMessageEventArgs message)
        {
            if (!string.IsNullOrEmpty(message.Message))
            {
                BattlEyeMessageReceived?.Invoke(message);
            }
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

        protected override void DisposeManagedResources()
        {
            var local = _battlEyeServer;
            _battlEyeServer = null;

            if (local != null)
            {
                lock (_lock)
                {
                    if (local.Connected)
                        local.Disconnect();
                    local.Dispose();
                }
            }
            base.DisposeManagedResources();

            Log.Info($"ThreadSafeBattleEyeClient DisposeManagedResources");
        }
    }
}