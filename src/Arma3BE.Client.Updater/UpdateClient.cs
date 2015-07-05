using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Updater.Models;
using BattleNET;

using Admin = Arma3BEClient.Updater.Models.Admin;

namespace Arma3BEClient.Updater
{
    public class UpdateClient : IDisposable
    {  
        private readonly string _host; 
        private readonly int _port;
        private readonly string _password;
        private readonly ILog _log;
        private BattlEyeClient _battlEyeClient;

        public bool Disposed { get; set; }

        private Thread _thread;

        private object _lock = new object();

        public event EventHandler<IEnumerable<Player>> PlayerHandler;
        public event EventHandler<IEnumerable<Ban>> BanHandler;
        public event EventHandler<IEnumerable<Admin>> AdminHandler;
        public event EventHandler<IEnumerable<Mission>> MissionHandler;

        public event EventHandler<ChatMessage> ChatMessageHandler;


        public event EventHandler<EventArgs> RConAdminLog;
        public event EventHandler<EventArgs> PlayerLog;
        public event EventHandler<EventArgs> BanLog;

        public event EventHandler ConnectHandler;
        public event EventHandler ConnectingHandler;
        public event EventHandler DisconnectHandler;


        protected virtual void OnMissionHandler(IEnumerable<Mission> e)
        {
            EventHandler<IEnumerable<Mission>> handler = MissionHandler;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnConnectingHandler()
        {
            EventHandler handler = ConnectingHandler;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnBanLog()
        {
            EventHandler<EventArgs> handler = BanLog;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnPlayerLog()
        {
            EventHandler<EventArgs> handler = PlayerLog;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnRConAdminLog()
        {
            EventHandler<EventArgs> handler = RConAdminLog;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected virtual void OnAdminHandler(IEnumerable<Admin> e)
        {
            EventHandler<IEnumerable<Admin>> handler = AdminHandler;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnChatMessageHandler(ChatMessage e)
        {
            EventHandler<ChatMessage> handler = ChatMessageHandler;
            if (handler != null) handler(this, e);
        }


        protected virtual void OnBanHandler(IEnumerable<Ban> e)
        {
            EventHandler<IEnumerable<Ban>> handler = BanHandler;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnDisconnectHandler()
        {
            EventHandler handler = DisconnectHandler;
            if (handler != null) handler(this, EventArgs.Empty);
        }


        protected virtual void OnConnectHandler()
        {
            EventHandler handler = ConnectHandler;
            if (handler != null) handler(this, EventArgs.Empty);
        }


        protected virtual void OnPlayerHandler(IEnumerable<Player> e)
        {
            EventHandler<IEnumerable<Player>> handler = PlayerHandler;
            if (handler != null) handler(this, e);
        }


        public UpdateClient(string host, int port, string password, ILog log)
        {
            _host = host;
            _port = port;
            _password = password;
            _log = log;

            InitClients();
        }

        void _battlEyeClient_BattlEyeDisconnected(BattlEyeDisconnectEventArgs args)
        {
            OnDisconnectHandler();
        }

        void battlEyeClient_BattlEyeMessageReceived(BattlEyeMessageEventArgs args)
        {
            try
            {
                var message = new ServerMessage(args.Id, args.Message);

                lock (_lock)
                {
                    ProcessMessage(message);
                }

            }
            catch (Exception e)
            {
                _log.Error(e);
            }

        }

        void battlEyeClient_BattlEyeConnected(BattlEyeConnectEventArgs args)
        {
            OnConnectHandler();
        }

        public bool Connected { get { return _battlEyeClient != null && _battlEyeClient.Connected; } }


        public Task SendCommandAsync(CommandType type, string parameters = null)
        {
            return Task.Run(()=>SendCommand(type, parameters));
        }


        public void SendCommand(CommandType type, string parameters = null)
        {
            lock (_lock)
            {
                _log.Info(string.Format("SERVER: {0}:{1} - TRY TO RCON COMMAND {2} WITH PARAMS {3}", _host, _port, type,
                    parameters));


                if (!Connected)
                {
                    Connect();
                    return;
                }

                switch (type)
                {
                    case CommandType.Players:
                        _battlEyeClient.SendCommand(BattlEyeCommand.Players);
                        break;
                    case CommandType.Bans:
                        _battlEyeClient.SendCommand(BattlEyeCommand.Bans);
                        break;
                    case CommandType.Admins:
                        _battlEyeClient.SendCommand(BattlEyeCommand.Admins);
                        break;

                    case CommandType.Say:
                        _battlEyeClient.SendCommand(BattlEyeCommand.Say, parameters);
                        break;
                    case CommandType.AddBan:
                        if (!string.IsNullOrEmpty(parameters))
                            _battlEyeClient.SendCommand(BattlEyeCommand.AddBan, parameters);
                        break;

                    case CommandType.Ban:
                        if (!string.IsNullOrEmpty(parameters))
                            _battlEyeClient.SendCommand(BattlEyeCommand.Ban, parameters);
                        break;

                    case CommandType.Kick:
                        if (!string.IsNullOrEmpty(parameters))
                            _battlEyeClient.SendCommand(BattlEyeCommand.Kick, parameters);
                        break;


                    case CommandType.RemoveBan:
                        if (!string.IsNullOrEmpty(parameters))
                            _battlEyeClient.SendCommand(BattlEyeCommand.RemoveBan, parameters);
                        break;

                    case CommandType.Missions:
                        _battlEyeClient.SendCommand(BattlEyeCommand.Missions);
                        break;

                    case CommandType.Mission:
                        if (!string.IsNullOrEmpty(parameters))
                            _battlEyeClient.SendCommand(BattlEyeCommand.Mission, parameters);
                        break;


                    case CommandType.Init:
                        _battlEyeClient.SendCommand(BattlEyeCommand.Init);
                        break;

                    case CommandType.Shutdown:
                        _battlEyeClient.SendCommand(BattlEyeCommand.Shutdown);
                        break;

                    case CommandType.Reassign:
                        _battlEyeClient.SendCommand(BattlEyeCommand.Reassign);
                        break;

                    case CommandType.Restart:
                        _battlEyeClient.SendCommand(BattlEyeCommand.Restart);
                        break;

                    case CommandType.Lock:
                        _battlEyeClient.SendCommand(BattlEyeCommand.Lock);
                        break;

                    case CommandType.Unlock:
                        _battlEyeClient.SendCommand(BattlEyeCommand.Unlock);
                        break;


                    case CommandType.LoadBans:
                        _battlEyeClient.SendCommand(BattlEyeCommand.LoadBans);
                        break;

                    case CommandType.LoadEvents:
                        _battlEyeClient.SendCommand(BattlEyeCommand.loadEvents);
                        break;

                    case CommandType.LoadScripts:
                        _battlEyeClient.SendCommand(BattlEyeCommand.LoadScripts);
                        break;

                    default:
                        break;
                }
            }
        }


        public enum CommandType
        {
            Players, Bans, Admins, Say, AddBan, Ban, Kick, RemoveBan,
            Init, Shutdown, Reassign, Restart, Lock, Unlock, Mission, Missions, RConPassword, MaxPing,
            LoadBans, LoadScripts, LoadEvents,
        };



        private void ProcessMessage(ServerMessage message)
        {
            switch (message.Type)
            {
                case ServerMessage.MessageType.PlayerList:
                    var list = new PlayerList(message);
                    OnPlayerHandler(list);
                    break;
                case ServerMessage.MessageType.BanList:
                    var banList = new BanList(message);
                    OnBanHandler(banList);
                    break;


                case ServerMessage.MessageType.AdminList:
                    var adminList = new AdminList(message);
                    OnAdminHandler(adminList);
                    break;

                case ServerMessage.MessageType.MissionList:
                    var missinlist = new MissionList(message);
                    OnMissionHandler(missinlist);
                    break;

                case ServerMessage.MessageType.ChatMessage:
                    var chatMessage = new ChatMessage()
                    {
                        Date = DateTime.UtcNow,
                        Message = message.Message
                    };

                    OnChatMessageHandler(chatMessage);
                    break;

                case ServerMessage.MessageType.RconAdminLog:
                    OnRConAdminLog();
                    OnChatMessageHandler(new ChatMessage { Date = DateTime.UtcNow, Message = message.Message });
                    break;


                case ServerMessage.MessageType.PlayerLog:
                    OnPlayerLog();
                    OnChatMessageHandler(new ChatMessage { Date = DateTime.UtcNow, Message = message.Message });
                    break;

                case ServerMessage.MessageType.BanLog:
                    OnBanLog();
                    OnChatMessageHandler(new ChatMessage { Date = DateTime.UtcNow, Message = message.Message });
                    break;

                case ServerMessage.MessageType.Unknown:
                    var unknownMessage = new ChatMessage()
                    {
                        Date = DateTime.UtcNow,
                        Message = message.Message
                    };

                    OnChatMessageHandler(unknownMessage);
                    break;
                default:
                    break;
            }

            RegisterMessage(message);
        }


        private void RegisterMessage(ServerMessage message)
        {
            _log.InfoFormat("message [\nserver ip: {0}\nmessageId:{1}\n{2}\n]", _host, message.MessageId, message.Message);
        }



        public void Connect()
        {
            _log.Info(string.Format("{0}:{1} Update client - connect", _host, _port));

            InitClients();

            if (_battlEyeClient != null && !_battlEyeClient.Connected)
            {
                OnConnectingHandler();
                if (_thread != null && _thread.IsAlive) _thread.Abort();
                _thread = new Thread((state) =>
                {
                    Thread.Sleep(100);
                    _battlEyeClient.Connect();
                }) { IsBackground = true };
                _thread.Start();
            }
        }




        private void InitClients()
        {
            _log.Info(string.Format("{0}:{1} Update client - InitClients", _host, _port));
            lock (_lock)
            {
                if (_battlEyeClient != null) ReleaseClient();

                var credentials = new BattlEyeLoginCredentials(IPAddress.Parse(_host), _port, _password);
                _battlEyeClient = new BattlEyeClient(credentials);
                _battlEyeClient.ReconnectOnPacketLoss = true;
                _battlEyeClient.BattlEyeConnected += battlEyeClient_BattlEyeConnected;
                _battlEyeClient.BattlEyeDisconnected += _battlEyeClient_BattlEyeDisconnected;
                _battlEyeClient.BattlEyeMessageReceived += battlEyeClient_BattlEyeMessageReceived;
            }
        }

        private void ReleaseClient()
        {
            _log.Info(string.Format("{0}:{1} Update client - ReleaseClient", _host, _port));
            lock (_lock)
            {
                if (_battlEyeClient != null)
                {
                    try
                    {
                        if (_battlEyeClient.Connected) _battlEyeClient.Disconnect();

                        _battlEyeClient.BattlEyeConnected -= battlEyeClient_BattlEyeConnected;
                        _battlEyeClient.BattlEyeDisconnected -= _battlEyeClient_BattlEyeDisconnected;
                        _battlEyeClient.BattlEyeMessageReceived -= battlEyeClient_BattlEyeMessageReceived;
                    }
                    finally
                    {
                        _battlEyeClient = null;
                    }
                }
            }
        }

        public void Disconnect()
        {
            _log.Info(string.Format("{0}:{1} Update client - Disconnect", _host, _port));
            try
            {
                ReleaseClient();
            }
            catch
            {
            }
        }

        public void Dispose()
        {
            try
            {
                Disposed = true;

                ReleaseClient();

                if (_thread != null) _thread.Abort();
                GC.SuppressFinalize(this);
            }
            finally
            {
                _battlEyeClient = null;
            }
        }

        ~UpdateClient()
        {
            if (!Disposed) Dispose();
        }
    }
}