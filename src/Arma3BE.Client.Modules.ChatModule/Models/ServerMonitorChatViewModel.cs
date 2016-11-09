using Arma3BE.Client.Infrastructure.Commands;
using Arma3BE.Client.Infrastructure.Events;
using Arma3BE.Client.Infrastructure.Events.BE;
using Arma3BE.Client.Infrastructure.Events.Models;
using Arma3BE.Client.Infrastructure.Extensions;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Client.Modules.ChatModule.Boxes;
using Arma3BE.Client.Modules.ChatModule.Helpers;
using Arma3BE.Server;
using Arma3BE.Server.Models;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Libs.ModelCompact;
using Arma3BEClient.Libs.Tools;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using Player = Arma3BE.Server.Models.Player;

namespace Arma3BE.Client.Modules.ChatModule.Models
{
    public class ServerMonitorChatViewModel : ViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ISettingsStoreSource _settingsStoreSource;
        private readonly ChatHelper _chatHelper;
        private readonly ILog _log;
        private readonly Guid _serverId;
        private bool _autoScroll;
        private bool _enableChat;
        private string _inputMessage;
        private List<Player> _players = new List<Player>();

        public ServerMonitorChatViewModel(ILog log, ServerInfo serverInfo, IEventAggregator eventAggregator, ISettingsStoreSource settingsStoreSource)
        {
            _log = log;
            _serverId = serverInfo.Id;
            _eventAggregator = eventAggregator;
            _settingsStoreSource = settingsStoreSource;

            AutoScroll = true;
            EnableChat = true;

            _chatHelper = new ChatHelper(_log, _serverId);

            _eventAggregator.GetEvent<BEMessageEvent<BEChatMessage>>()
                .Subscribe(BeServerChatMessageHandler, ThreadOption.UIThread);

            _eventAggregator.GetEvent<BEMessageEvent<BEAdminLogMessage>>()
                .Subscribe(_beServer_PlayerLog, ThreadOption.UIThread);
            _eventAggregator.GetEvent<BEMessageEvent<BEPlayerLogMessage>>()
                .Subscribe(_beServer_PlayerLog, ThreadOption.UIThread);
            _eventAggregator.GetEvent<BEMessageEvent<BEBanLogMessage>>()
                .Subscribe(_beServer_PlayerLog, ThreadOption.UIThread);

            _eventAggregator.GetEvent<BEMessageEvent<BEItemsMessage<Player>>>()
                .Subscribe(_beServer_PlayerHandler, ThreadOption.UIThread);

            var global = new Player(-1, null, 0, 0, null, "GLOBAL", Player.PlayerState.Ingame);
            Players = new List<Player> { global };
            SelectedPlayer = global;

            ShowHistoryCommand = new ActionCommand(() =>
            {
                var model = new ChatHistoryViewModel(_serverId);
                model.StartDate = DateTime.UtcNow.UtcToLocalFromSettings().AddHours(-5);
                var wnd = new ChatHistory(model);
                wnd.Show();
                wnd.Activate();
            });
        }




        private void _beServer_PlayerHandler(BEItemsMessage<Player> e)
        {
            if (e.ServerId != _serverId) return;

            var newItems = new List<Player>();
            var global = new Player(-1, null, 0, 0, null, "GLOBAL", Player.PlayerState.Ingame);
            newItems.Add(global);
            newItems.AddRange(e.Items.OrderBy(x => x.Name));

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

            ProcessPlayers(e);
        }



        Dictionary<string, string> _prev = new Dictionary<string, string>();
        private void ProcessPlayers(BEItemsMessage<Player> newPlayers)
        {
            if (newPlayers.ServerId != _serverId) return;

            var prev = _prev;
            var next = newPlayers.Items.GroupBy(x => x.Guid).ToDictionary(x => x.Key, x => x.First().Name);

            _prev = next;

            var addedItems = next.Where(x => prev.ContainsKey(x.Key) == false).ToArray();
            var removedItems = prev.Where(x => next.ContainsKey(x.Key) == false).ToArray();

            if (addedItems.Any())
                OnPlayersInHandler(addedItems);

            if (removedItems.Any())
                OnPlayersOutHandler(removedItems);
        }


        private void _beServer_PlayerLog(BEAdminLogMessage e)
        {
            if (_serverId == e.ServerId)
                OnLogMessageEventHandler(e.Message);
        }

        private void _beServer_PlayerLog(BEBanLogMessage e)
        {
            if (_serverId == e.ServerId)
                OnLogMessageEventHandler(e.Message);
        }

        private void _beServer_PlayerLog(BEPlayerLogMessage e)
        {
            if (_serverId == e.ServerId)
                OnLogMessageEventHandler(e.Message);
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

        public event EventHandler<IEnumerable<KeyValuePair<string, string>>> PlayersInHandler;
        public event EventHandler<IEnumerable<KeyValuePair<string, string>>> PlayersOutHandler;

        protected virtual void OnChatMessageEventHandler(ChatMessage e)
        {
            ChatMessageEventHandler?.Invoke(this, new ServerMonitorChatViewModelEventArgs(e));
        }

        private void BeServerChatMessageHandler(BEChatMessage e)
        {
            if (e.ServerId != _serverId) return;
            _chatHelper.RegisterChatMessage(e.Message);
            OnChatMessageEventHandler(e.Message);
        }

        public void SendMessage(string rawmessage)
        {
            if (!string.IsNullOrEmpty(rawmessage))
            {
                var adminName = _settingsStoreSource.GetSettingsStore().AdminName;

                var selectedPlayer = SelectedPlayer;
                var destinationNum = -1;
                if (selectedPlayer != null)
                {
                    destinationNum = selectedPlayer.Num;
                }

                var message = $" {destinationNum} {adminName}: {rawmessage}";

                _eventAggregator.GetEvent<BEMessageEvent<BECommand>>()
                    .Publish(new BECommand(_serverId, CommandType.Say, message));
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
                case ChatMessage.MessageType.NonCommon:
                    color = Color.FromRgb(89, 173, 10);
                    break;
                default:
                    break;
            }

            return color;
        }

        protected virtual void OnPlayersInHandler(IEnumerable<KeyValuePair<string, string>> e)
        {
            PlayersInHandler?.Invoke(this, e);
        }

        protected virtual void OnPlayersOutHandler(IEnumerable<KeyValuePair<string, string>> e)
        {
            PlayersOutHandler?.Invoke(this, e);
        }

        public void ShowPlayer(string guidIp)
        {
            if (string.IsNullOrEmpty(guidIp) == false)
                _eventAggregator.GetEvent<ShowUserInfoEvent>().Publish(new ShowUserModel(guidIp));
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