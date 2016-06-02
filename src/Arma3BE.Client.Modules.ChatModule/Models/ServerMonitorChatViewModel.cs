using Arma3BE.Client.Infrastructure.Commands;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Client.Modules.ChatModule.Boxes;
using Arma3BE.Client.Modules.ChatModule.Helpers;
using Arma3BE.Server;
using Arma3BE.Server.Abstract;
using Arma3BE.Server.Models;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Libs.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace Arma3BE.Client.Modules.ChatModule.Models
{
    public class ServerMonitorChatViewModel : ViewModelBase, IServerMonitorChatViewModel
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

        private void _beServer_PlayerHandler(object sender, BEClientEventArgs<System.Collections.Generic.IEnumerable<Player>> e)
        {
            var newItems = new List<Player>();
            var global = new Player(-1, null, 0, 0, null, "GLOBAL", Player.PlayerState.Ingame);
            newItems.Add(global);
            newItems.AddRange(e.Data.OrderBy(x => x.Name));

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
                OnPropertyChanged("Players");
            }
        }

        private Player _selectedPlayer;
        public Player SelectedPlayer
        {
            get { return _selectedPlayer; }
            set
            {
                _selectedPlayer = value;
                OnPropertyChanged("SelectedPlayer");
            }
        }

        public bool AutoScroll
        {
            get { return _autoScroll; }
            set
            {
                _autoScroll = value;
                OnPropertyChanged("AutoScroll");
            }
        }

        public bool EnableChat
        {
            get { return _enableChat; }
            set
            {
                _enableChat = value;
                OnPropertyChanged("EnableChat");
            }
        }

        public string InputMessage
        {
            get { return _inputMessage; }
            set
            {
                _inputMessage = value;
                OnPropertyChanged("InputMessage");
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

        public static Color GetMessageColor(ChatMessage message)
        {
            var type = message.Type;

            var color = Colors.Black;

            switch (type)
            {
                case ChatMessage.MessageType.Command:
                    color = Color.FromRgb(212, 169, 24);
                    break;
                case ChatMessage.MessageType.Direct:
                    color = Color.FromRgb(146, 140, 150);
                    break;
                case ChatMessage.MessageType.Global:
                    color = Color.FromRgb(80, 112, 115);
                    break;
                case ChatMessage.MessageType.Group:
                    color = Color.FromRgb(156, 204, 118);
                    break;
                case ChatMessage.MessageType.RCon:
                    color = Color.FromRgb(252, 31, 23);
                    break;
                case ChatMessage.MessageType.Side:
                    color = Color.FromRgb(25, 181, 209);
                    break;
                case ChatMessage.MessageType.Vehicle:
                    color = Color.FromRgb(155, 115, 0);
                    break;
                default:
                    break;
            }

            return color;
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