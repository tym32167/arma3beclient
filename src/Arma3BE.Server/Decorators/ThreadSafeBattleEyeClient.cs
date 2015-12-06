using System.Collections.Concurrent;
using System.Threading;
using Arma3BEClient.Common.Core;
using Arma3BEClient.Common.Logging;
using BattleNET;

namespace Arma3BE.Server.Decorators
{
    public class ThreadSafeBattleEyeClient : DisposeObject, IBattlEyeClient
    {
        private readonly ConcurrentQueue<CommandPacket> _commandPackets = new ConcurrentQueue<CommandPacket>();


        private readonly object _lock = new object();
        private readonly ILog _log;
        private readonly Timer _timer;
        private IBattlEyeClient _battlEyeClient;

        public ThreadSafeBattleEyeClient(IBattlEyeClient battlEyeClient, ILog log)
        {
            _battlEyeClient = battlEyeClient;
            _log = log;
            _battlEyeClient.BattlEyeConnected += OnBattlEyeConnected;
            _battlEyeClient.BattlEyeMessageReceived += OnBattlEyeMessageReceived;
            _battlEyeClient.BattlEyeDisconnected += OnBattlEyeDisconnected;

            _timer = new Timer(MainLoop, null, 1000, 1000);
            _log.Info($"ThreadSafeBattleEyeClient Init");
        }

        public bool Connected => _battlEyeClient.Connected;

        public int SendCommand(BattlEyeCommand command, string parameters = "")
        {
            if (_battlEyeClient != null && _battlEyeClient.Connected && _commandPackets.Count < 1000)
            {
                _commandPackets.Enqueue(new CommandPacket(command, parameters));
                _log.Info($"ThreadSafeBattleEyeClient Saving {command} WITH {parameters}");
            }
            return 0;
        }

        public void Disconnect()
        {
            lock (_lock)
            {
                _battlEyeClient?.Disconnect();
            }
        }

        public BattlEyeConnectionResult Connect()
        {
            lock (_lock)
            {
                return _battlEyeClient.Connect();
            }
        }

        public event BattlEyeMessageEventHandler BattlEyeMessageReceived;
        public event BattlEyeConnectEventHandler BattlEyeConnected;
        public event BattlEyeDisconnectEventHandler BattlEyeDisconnected;

        private void MainLoop(object state)
        {
            if (_battlEyeClient == null || !_battlEyeClient.Connected)
                return;

            if (!_commandPackets.IsEmpty && _battlEyeClient.Connected)
            {
                CommandPacket packet;

                if (_commandPackets.TryDequeue(out packet))
                {
                    lock (_lock)
                    {
                        if (_battlEyeClient != null && _battlEyeClient.Connected)
                        {
                            _battlEyeClient.SendCommand(packet.BattlEyeCommand, packet.Parameters);
                            _log.Info($"ThreadSafeBattleEyeClient Sending {packet.BattlEyeCommand} WITH {packet.Parameters}");
                        }
                    }
                }
            }
        }

        private void OnBattlEyeMessageReceived(BattlEyeMessageEventArgs message)
        {
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

        protected override void DisposeManagedResources()
        {
            _timer.Dispose();

            if (_battlEyeClient != null)
            {
                lock (_lock)
                {
                    if (_battlEyeClient != null)
                    {
                        if (_battlEyeClient.Connected)
                            _battlEyeClient.Disconnect();
                        _battlEyeClient.Dispose();
                        _battlEyeClient = null;
                    }
                }
            }
            base.DisposeManagedResources();

            _log.Info($"ThreadSafeBattleEyeClient DisposeManagedResources");
        }

        private class CommandPacket
        {
            public CommandPacket(BattlEyeCommand battlEyeCommand, string parameters)
            {
                BattlEyeCommand = battlEyeCommand;
                Parameters = parameters;
            }

            public BattlEyeCommand BattlEyeCommand { get; }
            public string Parameters { get; }
        }
    }
}