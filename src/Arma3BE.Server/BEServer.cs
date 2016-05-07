using System;
using System.Collections.Generic;
using System.Net;
using Arma3BE.Server.Abstract;
using Arma3BE.Server.Messaging;
using Arma3BE.Server.Models;
using Arma3BEClient.Common.Core;
using Arma3BEClient.Common.Logging;
using BattleNET;

namespace Arma3BE.Server
{
    public sealed class BEServer : DisposeObject, IBEServer
    {
        private readonly IBattlEyeServerFactory _battlEyeServerFactory;
        private readonly string _host;

        private readonly object _lock = new object();
        private readonly ILog _log;
        private readonly string _password;
        private readonly int _port;
        private IBattlEyeServer _battlEyeServer;


        public BEServer(string host, int port, string password, ILog log, IBattlEyeServerFactory battlEyeServerFactory)
        {
            _host = host;
            _port = port;
            _password = password;
            _log = log;
            _battlEyeServerFactory = battlEyeServerFactory;

            InitClients();
        }

        public bool Disposed { get; set; }

        public bool Connected
        {
            get { return _battlEyeServer != null && _battlEyeServer.Connected; }
        }

        public event EventHandler<UpdateClientEventArgs<IEnumerable<Player>>> PlayerHandler;
        public event EventHandler<UpdateClientEventArgs<IEnumerable<Ban>>> BanHandler;
        public event EventHandler<UpdateClientEventArgs<IEnumerable<Admin>>> AdminHandler;
        public event EventHandler<UpdateClientEventArgs<IEnumerable<Mission>>> MissionHandler;

        public event EventHandler<ChatMessage> ChatMessageHandler;


        public event EventHandler<LogMessage> RConAdminLog;
        public event EventHandler<LogMessage> PlayerLog;
        public event EventHandler<LogMessage> BanLog;

        public event EventHandler ConnectHandler;
        public event EventHandler ConnectingHandler;
        public event EventHandler DisconnectHandler;


        //public Task SendCommandAsync(CommandType type, string parameters = null)
        //{
        //    return Task.Run(() => SendCommand(type, parameters));
        //}


        public void SendCommand(CommandType type, string parameters = null)
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
                    _battlEyeServer.SendCommand(BattlEyeCommand.Players);
                    break;
                case CommandType.Bans:
                    _battlEyeServer.SendCommand(BattlEyeCommand.Bans);
                    break;
                case CommandType.Admins:
                    _battlEyeServer.SendCommand(BattlEyeCommand.admins);
                    break;

                case CommandType.Say:
                    _battlEyeServer.SendCommand(BattlEyeCommand.Say, parameters);
                    break;
                case CommandType.AddBan:
                    if (!string.IsNullOrEmpty(parameters))
                        _battlEyeServer.SendCommand(BattlEyeCommand.AddBan, parameters);
                    break;

                case CommandType.Ban:
                    if (!string.IsNullOrEmpty(parameters))
                        _battlEyeServer.SendCommand(BattlEyeCommand.Ban, parameters);
                    break;

                case CommandType.Kick:
                    if (!string.IsNullOrEmpty(parameters))
                        _battlEyeServer.SendCommand(BattlEyeCommand.Kick, parameters);
                    break;


                case CommandType.RemoveBan:
                    if (!string.IsNullOrEmpty(parameters))
                        _battlEyeServer.SendCommand(BattlEyeCommand.RemoveBan, parameters);
                    break;

                case CommandType.Missions:
                    _battlEyeServer.SendCommand(BattlEyeCommand.Missions);
                    break;

                case CommandType.Mission:
                    if (!string.IsNullOrEmpty(parameters))
                        _battlEyeServer.SendCommand(BattlEyeCommand.Mission, parameters);
                    break;


                case CommandType.Init:
                    _battlEyeServer.SendCommand(BattlEyeCommand.Init);
                    break;

                case CommandType.Shutdown:
                    _battlEyeServer.SendCommand(BattlEyeCommand.Shutdown);
                    break;

                case CommandType.Reassign:
                    _battlEyeServer.SendCommand(BattlEyeCommand.Reassign);
                    break;

                case CommandType.Restart:
                    _battlEyeServer.SendCommand(BattlEyeCommand.Restart);
                    break;

                case CommandType.Lock:
                    _battlEyeServer.SendCommand(BattlEyeCommand.Lock);
                    break;

                case CommandType.Unlock:
                    _battlEyeServer.SendCommand(BattlEyeCommand.Unlock);
                    break;


                case CommandType.LoadBans:
                    _battlEyeServer.SendCommand(BattlEyeCommand.LoadBans);
                    break;

                case CommandType.LoadEvents:
                    _battlEyeServer.SendCommand(BattlEyeCommand.loadEvents);
                    break;

                case CommandType.LoadScripts:
                    _battlEyeServer.SendCommand(BattlEyeCommand.LoadScripts);
                    break;
            }
        }

        public void Connect()
        {
            _log.Info($"{_host}:{_port} Update client - connect");

            _battlEyeServer?.Connect();
        }

        public void Disconnect()
        {
            _log.Info($"{_host}:{_port} Update client - Disconnect");
            _battlEyeServer?.Disconnect();
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

        private void OnBanLog(LogMessage message)
        {
            var handler = BanLog;
            handler?.Invoke(this, message);
        }

        private void OnPlayerLog(LogMessage message)
        {
            var handler = PlayerLog;
            handler?.Invoke(this, message);
        }

        private void OnRConAdminLog(LogMessage message)
        {
            var handler = RConAdminLog;
            handler?.Invoke(this, message);
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

        private void BattlEyeServerBattlEyeDisconnected(BattlEyeDisconnectEventArgs args)
        {
            OnDisconnectHandler();
        }

        private void BattlEyeServerBattlEyeMessageReceived(BattlEyeMessageEventArgs args)
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

        private void BattlEyeServerBattlEyeConnected(BattlEyeConnectEventArgs args)
        {
            OnConnectHandler();
        }


        private void ProcessMessage(ServerMessage message)
        {
            var logMessage = new LogMessage
            {
                Date = DateTime.UtcNow,
                Message = message.Message
            };

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
                    OnRConAdminLog(logMessage);
                    break;

                case ServerMessageType.PlayerLog:
                    OnPlayerLog(logMessage);
                    break;

                case ServerMessageType.BanLog:
                    OnBanLog(logMessage);
                    break;

                case ServerMessageType.Unknown:
                    //var unknownMessage = new ChatMessage
                    //{
                    //    Date = DateTime.UtcNow,
                    //    Message = message.Message
                    //};

                    //OnChatMessageHandler(unknownMessage);
                    break;
            }

            RegisterMessage(message);
        }


        private void RegisterMessage(ServerMessage message)
        {
            _log.Info($"message [\nserver ip: {_host}\nmessageId:{message.MessageId}\n{message.Message}\n]");
        }

        private void InitClients()
        {
            _log.Info($"{_host}:{_port} Update client - InitClients");
            lock (_lock)
            {
                if (_battlEyeServer != null) ReleaseClient();

                var credentials = new BattlEyeLoginCredentials(IPAddress.Parse(_host), _port, _password);
                _battlEyeServer = _battlEyeServerFactory.Create(credentials);
                _battlEyeServer.BattlEyeConnected += BattlEyeServerBattlEyeConnected;
                _battlEyeServer.BattlEyeDisconnected += BattlEyeServerBattlEyeDisconnected;
                _battlEyeServer.BattlEyeMessageReceived += BattlEyeServerBattlEyeMessageReceived;
            }
        }

        private void ReleaseClient()
        {
            _log.Info($"{_host}:{_port} Update client - ReleaseClient");
            lock (_lock)
            {
                if (_battlEyeServer != null)
                {
                    try
                    {
                        if (_battlEyeServer.Connected) _battlEyeServer.Disconnect();

                        _battlEyeServer.BattlEyeConnected -= BattlEyeServerBattlEyeConnected;
                        _battlEyeServer.BattlEyeDisconnected -= BattlEyeServerBattlEyeDisconnected;
                        _battlEyeServer.BattlEyeMessageReceived -= BattlEyeServerBattlEyeMessageReceived;

                        _battlEyeServer.Dispose();
                    }
                    finally
                    {
                        _battlEyeServer = null;
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
                _battlEyeServer = null;
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