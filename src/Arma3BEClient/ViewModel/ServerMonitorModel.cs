using System;
using System.Windows.Media;
using Arma3BEClient.Commands;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Helpers;
using Arma3BEClient.Libs.ModelCompact;
using Arma3BEClient.Models;
using Arma3BEClient.Updater;
using Arma3BEClient.Updater.Models;
using GalaSoft.MvvmLight;

namespace Arma3BEClient.ViewModel
{
    public class ServerMonitorModel : ViewModelBase
    {
        private readonly ServerInfo _currentServer;
        private readonly ILog _log;
        private readonly bool _console;
        private readonly UpdateClient _updateClient;
        
        private readonly UpdateClientPeriodic _updateClientPeriodic;
        

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



        public ServerMonitorModel(ServerInfo currentServer, ILog log, bool console = false)
        {
            _currentServer = currentServer;
            _log = log;
            _console = console;


            var host = IPInfo.GetIPAddress(_currentServer.Host);

            if (string.IsNullOrEmpty(host))
            {

                var message = string.Format("Host is incorrect for server {0}", _currentServer.Name);
                _log.Error(message);
                throw new Exception(message);
            }


            _updateClient = new UpdateClient(host, _currentServer.Port, _currentServer.Password, _log);
            
            _updateClient.PlayerHandler += (s, e) => PlayersViewModel.SetData(e);

            if (!console)
            {
                _updateClient.BanHandler += (s, e) => BansViewModel.SetData(e);
                _updateClient.AdminHandler += (s, e) => AdminsViewModel.SetData(e);
            }




            _updateClient.ConnectHandler += _updateClient_ConnectHandler;
            _updateClient.DisconnectHandler += _updateClient_DisconnectHandler;

            if (!console)
            {
                _updateClient.RConAdminLog += (s, e) => _updateClient.SendCommandAsync(UpdateClient.CommandType.Admins);
            }

            _updateClient.PlayerLog += (s, e) => _updateClient.SendCommandAsync(UpdateClient.CommandType.Players);


            if (!console)
            {
                _updateClient.BanLog += async (s, e) =>
                {
                    await _updateClient.SendCommandAsync(UpdateClient.CommandType.Players);
                    await _updateClient.SendCommandAsync(UpdateClient.CommandType.Bans);
                };
            }

            _updateClient.ConnectingHandler += (s, e) => RaisePropertyChanged("Connected");
            

            PlayersViewModel = new ServerMonitorPlayerViewModel(_log, currentServer, _updateClient);

            if (!console)
            {
                BansViewModel = new ServerMonitorBansViewModel(_log, currentServer.Id, _updateClient);
                AdminsViewModel = new ServerMonitorAdminsViewModel(_log, currentServer,
                    new ActionCommand(() => _updateClient.SendCommandAsync(UpdateClient.CommandType.Admins)));
                ManageServerViewModel = new ServerMonitorManageServerViewModel(_log, currentServer.Id, _updateClient);
                PlayerListModelView = new PlayerListModelView(_log, _updateClient, currentServer.Id);
            }

            ChatViewModel = new ServerMonitorChatViewModel(_log, currentServer.Id, _updateClient);
            _updateClientPeriodic = new UpdateClientPeriodic(_updateClient, log);
            _updateClientPeriodic.Start();
        }

        void _updateClient_DisconnectHandler(object sender, EventArgs e)
        {
            RaisePropertyChanged("Connected");
        }

        async void _updateClient_ConnectHandler(object sender, EventArgs e)
        {
            await _updateClient.SendCommandAsync(UpdateClient.CommandType.Players);
            if (!_console)
            {
                await _updateClient.SendCommandAsync(UpdateClient.CommandType.Bans);
                await _updateClient.SendCommandAsync(UpdateClient.CommandType.Admins);
                await _updateClient.SendCommandAsync(UpdateClient.CommandType.Missions);
            }

            RaisePropertyChanged("Connected");
        }

       
        
        public ServerInfo CurrentServer { get { return _currentServer; } }
        public bool Connected { get { return _updateClient.Connected; } }


        #region ViewModels


        public ServerMonitorPlayerViewModel PlayersViewModel { get; set; }
        public ServerMonitorBansViewModel BansViewModel { get; set; }
        public ServerMonitorAdminsViewModel AdminsViewModel { get; set; }

        public ServerMonitorChatViewModel ChatViewModel { get; set; }
        public ServerMonitorManageServerViewModel ManageServerViewModel { get; set; }

        public PlayerListModelView PlayerListModelView { get; set; }
       

        #endregion

        public void Connect()
        {
            if (!_updateClient.Connected)
                _updateClient.Connect();
        }

        
        public override void Cleanup()
        {
            base.Cleanup();
            _updateClientPeriodic.Dispose();
            _updateClient.Disconnect();
            _updateClient.Dispose();
        }
        
    }
}