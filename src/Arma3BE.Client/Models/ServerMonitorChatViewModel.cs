using System;
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
        private readonly ChatHelper _chatHelper;
        private readonly ILog _log;
        private readonly Guid _serverId;
        private readonly UpdateClient _updateClient;
        private bool _autoScroll;
        private bool _enableChat;
        private string _inputMessage;

        public ServerMonitorChatViewModel(ILog log, Guid serverId, UpdateClient updateClient)
        {
            _log = log;
            _serverId = serverId;
            _updateClient = updateClient;

            AutoScroll = true;
            EnableChat = true;

            _chatHelper = new ChatHelper(_log, _serverId);
            _updateClient.ChatMessageHandler += _updateClient_ChatMessageHandler;

            ShowHistoryCommand = new ActionCommand(() =>
            {
                var model = new ChatHistoryViewModel(serverId);
                model.StartDate = DateTime.UtcNow.AddHours(-5);
                var wnd = new ChatHistory(model);
                wnd.Show();
                wnd.Activate();
            });
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

        protected virtual void OnChatMessageEventHandler(ChatMessage e)
        {
            var handler = ChatMessageEventHandler;
            if (handler != null) handler(this, new ServerMonitorChatViewModelEventArgs(e));
        }

        private void _updateClient_ChatMessageHandler(object sender, ChatMessage e)
        {
            _chatHelper.RegisterChatMessage(e);
            OnChatMessageEventHandler(e);
        }

        public void SendMessage(string rawmessage)
        {
            if (!string.IsNullOrEmpty(rawmessage))
            {
                var adminName = SettingsStore.Instance.AdminName;
                var message = string.Format(" -1 {0}: {1}", adminName, rawmessage);
                _updateClient.SendCommandAsync(UpdateClient.CommandType.Say, message);
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
    }

    public class ServerMonitorChatViewModelEventArgs : EventArgs
    {
        public ServerMonitorChatViewModelEventArgs(ChatMessage message)
        {
            Message = message;
        }

        public ChatMessage Message { get; private set; }
    }
}