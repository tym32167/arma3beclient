using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Arma3BEClient.Common.Logging;
using BattleNET;

namespace Arma3BE.Server
{
    public interface IBattlEyeClientFactory
    {
        IBattlEyeClient Create(BattlEyeLoginCredentials credentials);
    }

    public class BattlEyeClientFactory : IBattlEyeClientFactory
    {
        private readonly ILog _log;

        public BattlEyeClientFactory(ILog log)
        {
            _log = log;
        }

        public IBattlEyeClient Create(BattlEyeLoginCredentials credentials)
        {
            return new ThreadSafeBattleEyeClient(new BattlEyeClient(credentials), _log);
        }
    }

    public class ThreadSafeBattleEyeClient : IBattlEyeClient
    {
        private readonly IBattlEyeClient _battlEyeClient;
        private readonly ILog _log;
        private ConcurrentQueue<CommandPacket>  _commandPackets = new ConcurrentQueue<CommandPacket>();
        private Thread ProcessThread;


        private static int Instances = 0;

        private void MainLoop()
        {
            while (true)
            {
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

        public ThreadSafeBattleEyeClient(IBattlEyeClient battlEyeClient, ILog log)
        {
            _battlEyeClient = battlEyeClient;
            _log = log;
            _battlEyeClient.BattlEyeConnected += OnBattlEyeConnected;
            _battlEyeClient.BattlEyeMessageReceived += OnBattlEyeMessageReceived;
            _battlEyeClient.BattlEyeDisconnected += OnBattlEyeDisconnected;

            

            Instances++;
        }

        public bool Connected => _battlEyeClient.Connected;

        public bool ReconnectOnPacketLoss
        {
            get { return _battlEyeClient.ReconnectOnPacketLoss; }
            set { _battlEyeClient.ReconnectOnPacketLoss = value; } 
        }

        public int SendCommand(BattlEyeCommand command, string parameters = "")
        {

            _commandPackets.Enqueue(new CommandPacket(command, parameters));
            //_battlEyeClient.SendCommand(command, parameters);
            return 0;
        }

        public void Disconnect()
        {
            ProcessThread.Abort();
            _battlEyeClient.Disconnect();
        }

        public BattlEyeConnectionResult Connect()
        {
            ProcessThread?.Abort();

            ProcessThread = new Thread(MainLoop);
            ProcessThread.Start();
            return _battlEyeClient.Connect();
        }

        public event BattlEyeMessageEventHandler BattlEyeMessageReceived;
        public event BattlEyeConnectEventHandler BattlEyeConnected;
        public event BattlEyeDisconnectEventHandler BattlEyeDisconnected;

        protected virtual void OnBattlEyeMessageReceived(BattlEyeMessageEventArgs message)
        {
            _log.InfoFormat("message [\nmessage id: {0}\nmessage:{1}\n]", message.Id, message.Message);


            BattlEyeMessageReceived?.Invoke(message);
        }

        protected virtual void OnBattlEyeConnected(BattlEyeConnectEventArgs args)
        {
            BattlEyeConnected?.Invoke(args);
        }

        protected virtual void OnBattlEyeDisconnected(BattlEyeDisconnectEventArgs args)
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