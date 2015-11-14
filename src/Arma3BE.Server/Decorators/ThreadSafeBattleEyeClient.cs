using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Threading;
using Arma3BEClient.Common.Core;
using Arma3BEClient.Common.Logging;
using BattleNET;

namespace Arma3BE.Server.Decorators
{
    public class ThreadSafeBattleEyeClient : DisposeObject, IBattlEyeClient
    {
        private readonly IBattlEyeClient _battlEyeClient;
        private readonly ILog _log;
        private readonly ConcurrentQueue<CommandPacket> _commandPackets = new ConcurrentQueue<CommandPacket>();
        private Thread _processThread;

        public ThreadSafeBattleEyeClient(IBattlEyeClient battlEyeClient, ILog log)
        {
            _battlEyeClient = battlEyeClient;
            _log = log;
            _battlEyeClient.BattlEyeConnected += OnBattlEyeConnected;
            _battlEyeClient.BattlEyeMessageReceived += OnBattlEyeMessageReceived;
            _battlEyeClient.BattlEyeDisconnected += OnBattlEyeDisconnected;
        }

        public bool Connected => _battlEyeClient.Connected
                                 && _processThread != null && _processThread.IsAlive;

        public bool ReconnectOnPacketLoss
        {
            get { return _battlEyeClient.ReconnectOnPacketLoss; }
            set { _battlEyeClient.ReconnectOnPacketLoss = value; }
        }

        public int SendCommand(BattlEyeCommand command, string parameters = "")
        {
            if (_commandPackets.Count < 10)
                _commandPackets.Enqueue(new CommandPacket(command, parameters));
            return 0;
        }

        public void Disconnect()
        {
            if (_processThread != null && _processThread.IsAlive) _processThread.Abort();
            _battlEyeClient?.Disconnect();
        }

        public BattlEyeConnectionResult Connect()
        {
            _processThread?.Abort();

            _processThread = new Thread(MainLoop) { IsBackground = true };
            _processThread.Start();
            return _battlEyeClient.Connect();
        }

        public event BattlEyeMessageEventHandler BattlEyeMessageReceived;
        public event BattlEyeConnectEventHandler BattlEyeConnected;
        public event BattlEyeDisconnectEventHandler BattlEyeDisconnected;

        private void MainLoop()
        {
            while (true)
            {
                if (_battlEyeClient == null || !_battlEyeClient.Connected)
                    continue;

                if (!_commandPackets.IsEmpty && _battlEyeClient.Connected)
                {
                    CommandPacket packet;
                    if (_commandPackets.TryDequeue(out packet))
                    {
                        _battlEyeClient.SendCommand(packet.BattlEyeCommand, packet.Parameters);
                    }
                }

                Thread.Sleep(300);
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