using Arma3BE.Server.Abstract;
using Arma3BEClient.Common.Core;
using Arma3BEClient.Common.Logging;
using BattleNET;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Arma3BE.Server.ServerDecorators
{
    public class ThreadSafeBattleEyeServer : DisposeObject, IBattlEyeServer
    {
        private readonly ConcurrentQueue<ICommandPacket> _commandPackets = new ConcurrentQueue<ICommandPacket>();
        private readonly ConcurrentQueue<BattlEyeMessageEventArgs> _messages = new ConcurrentQueue<BattlEyeMessageEventArgs>();

        private readonly object _lock = new object();
        private readonly ILog _log = LogFactory.Create(new StackTrace().GetFrame(0).GetMethod().DeclaringType);
        private readonly Timer _timer;
        private IBattlEyeServer _battlEyeServer;

        public ThreadSafeBattleEyeServer(IBattlEyeServer battlEyeServer)
        {
            _battlEyeServer = battlEyeServer;
            _battlEyeServer.BattlEyeConnected += OnBattlEyeConnected;
            _battlEyeServer.BattlEyeMessageReceived += OnBattlEyeMessageReceived;
            _battlEyeServer.BattlEyeDisconnected += OnBattlEyeDisconnected;

            _timer = new Timer(Process, null, 1000, 1000);
            _log.Info($"ThreadSafeBattleEyeClient Init");
        }

        public bool Connected => _battlEyeServer.Connected;

        public int SendCommand(BattlEyeCommand command, string parameters = "")
        {
            if (_battlEyeServer != null && _battlEyeServer.Connected && _commandPackets.Count < 1000)
            {
                _commandPackets.Enqueue(new CommandPacket(command, parameters));
                _log.Info($"ThreadSafeBattleEyeClient Saving {command} WITH {parameters}");
            }
            return 0;
        }

        public int SendCommand(string command)
        {
            if (_battlEyeServer != null && _battlEyeServer.Connected && _commandPackets.Count < 1000)
            {
                _commandPackets.Enqueue(new PureCommandPacket(command));
                _log.Info($"ThreadSafeBattleEyeClient Saving {command}");
            }
            return 0;
        }

        public void Disconnect()
        {
            lock (_lock)
            {
                _battlEyeServer?.Disconnect();
            }
        }

        public BattlEyeConnectionResult Connect()
        {
            lock (_lock)
            {
                _log.Info($"TRY TO CONNECT TO");
                return _battlEyeServer.Connect();
            }
        }

        public event BattlEyeMessageEventHandler BattlEyeMessageReceived;
        public event BattlEyeConnectEventHandler BattlEyeConnected;
        public event BattlEyeDisconnectEventHandler BattlEyeDisconnected;


        private void Process(object state)
        {
            Task.Factory.StartNew(ProcessRecieveMessages);
            Task.Factory.StartNew(ProcessSendMessages);
        }

        private void ProcessRecieveMessages()
        {
            BattlEyeMessageEventArgs message;
            if (_messages.TryDequeue(out message))
            {
                lock (_lock)
                {
                    BattlEyeMessageReceived?.Invoke(message);
                }
            }
        }

        private void ProcessSendMessages()
        {
            if (_battlEyeServer == null || !_battlEyeServer.Connected)
                return;

            if (!_commandPackets.IsEmpty && _battlEyeServer.Connected)
            {
                ICommandPacket packet;

                if (_commandPackets.TryDequeue(out packet))
                {
                    lock (_lock)
                    {
                        if (_battlEyeServer != null && _battlEyeServer.Connected)
                        {
                            packet.Invoke(_battlEyeServer);
                            //_battlEyeServer.SendCommand(packet.BattlEyeCommand, packet.Parameters);
                            _log.Info($"ThreadSafeBattleEyeClient Sending {packet}");
                        }
                    }
                }
            }
        }

        private void OnBattlEyeMessageReceived(BattlEyeMessageEventArgs message)
        {
            if (!string.IsNullOrEmpty(message.Message) && _messages.Count < 1000)
                _messages.Enqueue(message);
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

            if (_battlEyeServer != null)
            {
                lock (_lock)
                {
                    if (_battlEyeServer != null)
                    {
                        if (_battlEyeServer.Connected)
                            _battlEyeServer.Disconnect();
                        _battlEyeServer.Dispose();
                        _battlEyeServer = null;
                    }
                }
            }
            base.DisposeManagedResources();

            _log.Info($"ThreadSafeBattleEyeClient DisposeManagedResources");
        }

        private interface ICommandPacket
        {
            void Invoke(IBattlEyeServer server);
        }

        private class PureCommandPacket : ICommandPacket
        {
            private readonly string _command;

            public PureCommandPacket(string command)
            {
                _command = command;
            }

            public void Invoke(IBattlEyeServer server)
            {
                server?.SendCommand(_command);
            }

            public override string ToString()
            {
                return _command;
            }
        }

        private class CommandPacket : ICommandPacket
        {
            public CommandPacket(BattlEyeCommand battlEyeCommand, string parameters)
            {
                BattlEyeCommand = battlEyeCommand;
                Parameters = parameters;
            }

            public BattlEyeCommand BattlEyeCommand { get; }
            public string Parameters { get; }
            public void Invoke(IBattlEyeServer server)
            {
                server?.SendCommand(BattlEyeCommand, Parameters);
            }

            public override string ToString()
            {
                return $"{BattlEyeCommand} WITH {Parameters}";
            }
        }
    }
}