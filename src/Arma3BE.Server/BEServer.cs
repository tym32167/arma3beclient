using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Arma3BE.Server.Messaging;
using Arma3BE.Server.Models;
using Arma3BEClient.Common.Core;
using Arma3BEClient.Common.Logging;
using BattleNET;

namespace Arma3BE.Server
{
    public sealed class BEServer : DisposeObject, IBEServer
    {
        private readonly string _host;

        private readonly object _lock = new object();
        private readonly ILog _log;
        private readonly IBattlEyeClientFactory _battlEyeClientFactory;
        private readonly string _password;
        private readonly int _port;
        private IBattlEyeClient _battlEyeClient;


        public BEServer(string host, int port, string password, ILog log, IBattlEyeClientFactory battlEyeClientFactory)
        {
            _host = host;
            _port = port;
            _password = password;
            _log = log;
            _battlEyeClientFactory = battlEyeClientFactory;

           InitClients();
        }

        public bool Disposed { get; set; }

        public bool Connected
        {
            get { return _battlEyeClient != null && _battlEyeClient.Connected; }
        }

        public event EventHandler<UpdateClientEventArgs<IEnumerable<Player>>> PlayerHandler;
        public event EventHandler<UpdateClientEventArgs<IEnumerable<Ban>>> BanHandler;
        public event EventHandler<UpdateClientEventArgs<IEnumerable<Admin>>> AdminHandler;
        public event EventHandler<UpdateClientEventArgs<IEnumerable<Mission>>> MissionHandler;

        public event EventHandler<ChatMessage> ChatMessageHandler;


        public event EventHandler<EventArgs> RConAdminLog;
        public event EventHandler<EventArgs> PlayerLog;
        public event EventHandler<EventArgs> BanLog;

        public event EventHandler ConnectHandler;
        public event EventHandler ConnectingHandler;
        public event EventHandler DisconnectHandler;


        public Task SendCommandAsync(CommandType type, string parameters = null)
        {
            return Task.Run(() => SendCommand(type, parameters));
        }


        public void SendCommand(CommandType type, string parameters = null)
        {
            lock (_lock)
            {
                _log.Info($"SERVER: {_host}:{_port} - TRY TO RCON COMMAND {type} WITH PARAMS {parameters}");


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
                        _battlEyeClient.SendCommand(BattlEyeCommand.admins);
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
                }
            }
        }


        public void Connect()
        {
            _log.Info($"{_host}:{_port} Update client - connect");

            _battlEyeClient.Connect();
        }

        public void Disconnect()
        {
            _log.Info($"{_host}:{_port} Update client - Disconnect");
            _battlEyeClient.Disconnect();
        }


        private void OnMissionHandler(IEnumerable<Mission> e)
        {
            var handler = MissionHandler;
            handler?.Invoke(this, new UpdateClientEventArgs<IEnumerable<Mission>>(e));
        }

        private void OnConnectingHandler()
        {
            var handler = ConnectingHandler;
            handler?.Invoke(this, EventArgs.Empty);
        }

        private void OnBanLog()
        {
            var handler = BanLog;
            handler?.Invoke(this, EventArgs.Empty);
        }

        private void OnPlayerLog()
        {
            var handler = PlayerLog;
            handler?.Invoke(this, EventArgs.Empty);
        }

        private void OnRConAdminLog()
        {
            var handler = RConAdminLog;
            handler?.Invoke(this, EventArgs.Empty);
        }

        private void OnAdminHandler(IEnumerable<Admin> e)
        {
            var handler = AdminHandler;
            handler?.Invoke(this, new UpdateClientEventArgs<IEnumerable<Admin>>(e));
        }

        private void OnChatMessageHandler(ChatMessage e)
        {
            var handler = ChatMessageHandler;
            handler?.Invoke(this, e);
        }


        private void OnBanHandler(IEnumerable<Ban> e)
        {
            var handler = BanHandler;
            handler?.Invoke(this, new UpdateClientEventArgs<IEnumerable<Ban>>(e));
        }

        private void OnDisconnectHandler()
        {
            var handler = DisconnectHandler;
            handler?.Invoke(this, EventArgs.Empty);
        }


        private void OnConnectHandler()
        {
            var handler = ConnectHandler;
            handler?.Invoke(this, EventArgs.Empty);
        }


        private void OnPlayerHandler(IEnumerable<Player> e)
        {
            var handler = PlayerHandler;
            handler?.Invoke(this, new UpdateClientEventArgs<IEnumerable<Player>>(e));
        }

        private void _battlEyeClient_BattlEyeDisconnected(BattlEyeDisconnectEventArgs args)
        {
            OnDisconnectHandler();
        }

        private void battlEyeClient_BattlEyeMessageReceived(BattlEyeMessageEventArgs args)
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

        private void battlEyeClient_BattlEyeConnected(BattlEyeConnectEventArgs args)
        {
            OnConnectHandler();
        }


        private void ProcessMessage(ServerMessage message)
        {
            switch (message.Type)
            {
                case ServerMessageType.PlayerList:
                    var list = new PlayerList(message);
                    OnPlayerHandler(list);
                    break;
                case ServerMessageType.BanList:
                    var banList = new BanList(message);
                    OnBanHandler(banList);
                    break;


                case ServerMessageType.AdminList:
                    var adminList = new AdminList(message);
                    OnAdminHandler(adminList);
                    break;

                case ServerMessageType.MissionList:
                    var missinlist = new MissionList(message);
                    OnMissionHandler(missinlist);
                    break;

                case ServerMessageType.ChatMessage:
                    var chatMessage = new ChatMessage
                    {
                        Date = DateTime.UtcNow,
                        Message = message.Message
                    };

                    OnChatMessageHandler(chatMessage);
                    break;

                case ServerMessageType.RconAdminLog:
                    OnRConAdminLog();
                    OnChatMessageHandler(new ChatMessage {Date = DateTime.UtcNow, Message = message.Message});
                    break;


                case ServerMessageType.PlayerLog:
                    OnPlayerLog();
                    OnChatMessageHandler(new ChatMessage {Date = DateTime.UtcNow, Message = message.Message});
                    break;

                case ServerMessageType.BanLog:
                    OnBanLog();
                    OnChatMessageHandler(new ChatMessage {Date = DateTime.UtcNow, Message = message.Message});
                    break;

                case ServerMessageType.Unknown:
                    var unknownMessage = new ChatMessage
                    {
                        Date = DateTime.UtcNow,
                        Message = message.Message
                    };

                    OnChatMessageHandler(unknownMessage);
                    break;
            }

            RegisterMessage(message);
        }


        private void RegisterMessage(ServerMessage message)
        {
            //_log.InfoFormat("message [\nserver ip: {0}\nmessageId:{1}\n{2}\n]", _host, message.MessageId,
            //    message.Message);
        }


        private void InitClients()
        {
            _log.Info($"{_host}:{_port} Update client - InitClients");
            lock (_lock)
            {
                if (_battlEyeClient != null) ReleaseClient();

                var credentials = new BattlEyeLoginCredentials(IPAddress.Parse(_host), _port, _password);
                _battlEyeClient = _battlEyeClientFactory.Create(credentials);  
                _battlEyeClient.ReconnectOnPacketLoss = true;
                _battlEyeClient.BattlEyeConnected += battlEyeClient_BattlEyeConnected;
                _battlEyeClient.BattlEyeDisconnected += _battlEyeClient_BattlEyeDisconnected;
                _battlEyeClient.BattlEyeMessageReceived += battlEyeClient_BattlEyeMessageReceived;
            }
        }

        private void ReleaseClient()
        {
            _log.Info($"{_host}:{_port} Update client - ReleaseClient");
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


        protected override void DisposeUnManagedResources()
        {
            base.DisposeUnManagedResources();
            try
            {
                Disposed = true;
                ReleaseClient();
                GC.SuppressFinalize(this);
            }
            finally
            {
                _battlEyeClient = null;
            }
        }
    }


    public class UpdateClientEventArgs<T> : EventArgs
    {
        public UpdateClientEventArgs(T data)
        {
            Data = data;
        }

        public T Data { get; private set; }
    }
}