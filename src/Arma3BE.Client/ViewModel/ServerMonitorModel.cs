using System;
using System.Windows.Media;
using Arma3BE.Server;
using Arma3BE.Server.Models;
using Arma3BEClient.Commands;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Contracts;
using Arma3BEClient.Helpers;
using Arma3BEClient.Libs.ModelCompact;
using Arma3BEClient.Models;

namespace Arma3BEClient.ViewModel
{
    public class ServerMonitorModel : DisposableViewModelBase
    {
        private readonly IBEServer _beServer;
        private readonly bool _console;
        private readonly ILog _log;
        private readonly UpdateClientPeriodic _updateClientPeriodic;

        public ServerMonitorModel(ServerInfo currentServer, ILog log, bool console = false)
        {
            CurrentServer = currentServer;
            _log = log;
            _console = console;


            var host = IPInfo.GetIPAddress(CurrentServer.Host);

            if (string.IsNullOrEmpty(host))
            {
                var message = string.Format("Host is incorrect for server {0}", CurrentServer.Name);
                _log.Error(message);
                throw new Exception(message);
            }


            SteamQueryViewModel = new ServerMonitorSteamQueryViewModel(CurrentServer.Host, CurrentServer.Port, _log);

            _beServer = new BEServer(host, CurrentServer.Port, CurrentServer.Password, _log, new BattlEyeClientFactory(_log));

            _beServer.PlayerHandler += (s, e) => PlayersViewModel.SetData(e.Data);

            if (!console)
            {
                _beServer.BanHandler += (s, e) => BansViewModel.SetData(e.Data);
                _beServer.AdminHandler += (s, e) => AdminsViewModel.SetData(e.Data);
            }


            _beServer.ConnectHandler += BeServerConnectHandler;
            _beServer.DisconnectHandler += BeServerDisconnectHandler;

            if (!console)
            {
                _beServer.RConAdminLog += (s, e) => _beServer.SendCommandAsync(CommandType.Admins);
            }

            _beServer.PlayerLog += (s, e) => _beServer.SendCommandAsync(CommandType.Players);


            if (!console)
            {
                _beServer.BanLog += async (s, e) =>
                {
                    await _beServer.SendCommandAsync(CommandType.Players);
                    await _beServer.SendCommandAsync(CommandType.Bans);
                };
            }

            _beServer.ConnectingHandler += (s, e) => RaisePropertyChanged("Connected");


            PlayersViewModel = new ServerMonitorPlayerViewModel(_log, currentServer, _beServer);

            if (!console)
            {
                BansViewModel = new ServerMonitorBansViewModel(_log, currentServer.Id, _beServer);
                AdminsViewModel = new ServerMonitorAdminsViewModel(_log, currentServer,
                    new ActionCommand(() => _beServer.SendCommandAsync(CommandType.Admins)));
                ManageServerViewModel = new ServerMonitorManageServerViewModel(_log, currentServer.Id, _beServer);
                PlayerListModelView = new PlayerListModelView(_log, _beServer, currentServer.Id);
            }

            ChatViewModel = new ServerMonitorChatViewModel(_log, currentServer.Id, _beServer);
            _updateClientPeriodic = new UpdateClientPeriodic(_beServer, log);
            _updateClientPeriodic.Start();
        }

        public ServerInfo CurrentServer { get; }

        public bool Connected
        {
            get { return _beServer.Connected; }
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

        private void BeServerDisconnectHandler(object sender, EventArgs e)
        {
            RaisePropertyChanged("Connected");
        }

        private async void BeServerConnectHandler(object sender, EventArgs e)
        {
            await _beServer.SendCommandAsync(CommandType.Players);
            if (!_console)
            {
                await _beServer.SendCommandAsync(CommandType.Bans);
                await _beServer.SendCommandAsync(CommandType.Admins);
                await _beServer.SendCommandAsync(CommandType.Missions);
            }

            RaisePropertyChanged("Connected");
        }

        public void Connect()
        {
            try
            {
                if (!_beServer.Connected)
                    _beServer.Connect();
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();
            _updateClientPeriodic.Dispose();
            _beServer.Disconnect();
            _beServer.Dispose();
        }

        #region ViewModels

        public ServerMonitorSteamQueryViewModel SteamQueryViewModel { get; set; }

        public ServerMonitorPlayerViewModel PlayersViewModel { get; set; }
        public ServerMonitorBansViewModel BansViewModel { get; set; }
        public ServerMonitorAdminsViewModel AdminsViewModel { get; set; }

        public ServerMonitorChatViewModel ChatViewModel { get; set; }
        public ServerMonitorManageServerViewModel ManageServerViewModel { get; set; }

        public PlayerListModelView PlayerListModelView { get; set; }

        #endregion
    }
}