using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Arma3BE.Server;
using Arma3BE.Server.Models;
using Arma3BEClient.Boxes;
using Arma3BEClient.Commands;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Helpers;
using Arma3BEClient.Libs.Tools;
using GalaSoft.MvvmLight;

namespace Arma3BEClient.Models
{
    public class ServerMonitorChatViewModel : ViewModelBase
    {
        private readonly IBEServer _beServer;
        private readonly ChatHelper _chatHelper;
        private readonly ILog _log;
        private readonly Guid _serverId;
        private bool _autoScroll;
        private bool _enableChat;
        private string _inputMessage;
        private List<Player> _players = new List<Player>();

        public ServerMonitorChatViewModel(ILog log, Guid serverId, IBEServer beServer)
        {
            _log = log;
            _serverId = serverId;
            _beServer = beServer;

            AutoScroll = true;
            EnableChat = true;

            _chatHelper = new ChatHelper(_log, _serverId);
            _beServer.ChatMessageHandler += BeServerChatMessageHandler;
            _beServer.PlayerLog += _beServer_PlayerLog;
            _beServer.RConAdminLog += _beServer_PlayerLog;
            _beServer.BanLog += _beServer_PlayerLog;
            _beServer.PlayerHandler += _beServer_PlayerHandler;
            
            var global = new Player(-1, null, 0, 0, null, "GLOBAL", Player.PlayerState.Ingame);
            Players = new List<Player> { global }; 
            SelectedPlayer = global;

            ShowHistoryCommand = new ActionCommand(() =>
            {
                var model = new ChatHistoryViewModel(serverId);
                model.StartDate = DateTime.UtcNow.AddHours(-5);
                var wnd = new ChatHistory(model);
                wnd.Show();
                wnd.Activate();
            });
        }

        private void _beServer_PlayerHandler(object sender, UpdateClientEventArgs<System.Collections.Generic.IEnumerable<Player>> e)
        {
            var newItems = new List<Player>();
            var global = new Player(-1, null, 0, 0, null, "GLOBAL", Player.PlayerState.Ingame);
            newItems.Add(global);
            newItems.AddRange(e.Data.OrderBy(x=>x.Name));

            var selected = SelectedPlayer;

            Players = newItems;

            if (selected != null)
            {
                var newSelected = Players.FirstOrDefault(x => x.Num == selected.Num && x.Name == selected.Name);
                if (newSelected != null) SelectedPlayer = newSelected;
            }
            else
            {
                SelectedPlayer = global;
            }
        }

        private void _beServer_PlayerLog(object sender, LogMessage e)
        {
            OnLogMessageEventHandler(e);
        }

        public List<Player> Players
        {
            get { return _players; }
            set
            {
                _players = value;
                RaisePropertyChanged("Players");
            }
        }

        private Player _selectedPlayer;
        public Player SelectedPlayer
        {
            get { return _selectedPlayer; }
            set
            {
                _selectedPlayer = value;
                RaisePropertyChanged("SelectedPlayer");
            }
        }

        public bool AutoScroll
        {
            get { return _autoScroll; }
            set
            {
                _autoScroll = value;
                RaisePropertyChanged("AutoScroll");
            }
        }

        public bool EnableChat
        {
            get { return _enableChat; }
            set
            {
                _enableChat = value;
                RaisePropertyChanged("EnableChat");
            }
        }

        public string InputMessage
        {
            get { return _inputMessage; }
            set
            {
                _inputMessage = value;
                RaisePropertyChanged("InputMessage");
            }
        }

        public ICommand ShowHistoryCommand { get; set; }
        public event EventHandler<ServerMonitorChatViewModelEventArgs> ChatMessageEventHandler;
        public event EventHandler<ServerMonitorLogViewModelEventArgs> LogMessageEventHandler;

        protected virtual void OnChatMessageEventHandler(ChatMessage e)
        {
            var handler = ChatMessageEventHandler;
            if (handler != null) handler(this, new ServerMonitorChatViewModelEventArgs(e));
        }

        private void BeServerChatMessageHandler(object sender, ChatMessage e)
        {
            _chatHelper.RegisterChatMessage(e);
            OnChatMessageEventHandler(e);
        }

        public void SendMessage(string rawmessage)
        {
            if (!string.IsNullOrEmpty(rawmessage))
            {
                var adminName = SettingsStore.Instance.AdminName;

                var selectedPlayer = SelectedPlayer;
                var destinationNum = -1;
                if (selectedPlayer != null)
                {
                    destinationNum = selectedPlayer.Num;
                }

                var message = $" {destinationNum} {adminName}: {rawmessage}";
                _beServer.SendCommand(CommandType.Say, message);
            }

            InputMessage = string.Empty;
        }

        public void SendMessage()
        {
            if (!string.IsNullOrEmpty(InputMessage))
            {
                SendMessage(InputMessage);
            }
        }

        protected virtual void OnLogMessageEventHandler(LogMessage e)
        {
            LogMessageEventHandler?.Invoke(this, new ServerMonitorLogViewModelEventArgs(e));
        }
    }

    public class ServerMonitorChatViewModelEventArgs : EventArgs
    {
        public ServerMonitorChatViewModelEventArgs(ChatMessage message)
        {
            Message = message;
        }

        public ChatMessage Message { get; private set; }
    }

    public class ServerMonitorLogViewModelEventArgs : EventArgs
    {
        public ServerMonitorLogViewModelEventArgs(LogMessage message)
        {
            Message = message;
        }

        public LogMessage Message { get; private set; }
    }
}