using Arma3BE.Client.Infrastructure;
using Arma3BE.Client.Infrastructure.Contracts;
using Arma3BE.Client.Infrastructure.Events;
using Arma3BE.Client.Infrastructure.Events.BE;
using Arma3BE.Client.Infrastructure.Events.Models;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Server;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Libs.ModelCompact;
using Microsoft.Practices.Unity;
using Prism.Events;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Arma3BE.Client.Modules.MainModule.ViewModel
{
    public class ServerMonitorModel : DisposableViewModelBase
    {
        //private IBEServer _beServer;
        private readonly bool _console;
        private readonly ILog _log;
        private readonly IEventAggregator _eventAggregator;
        private bool _isBusy;

        public ServerMonitorModel(ServerInfo currentServer, ILog log, IIpService ipService, IUnityContainer container, IEventAggregator eventAggregator, bool console = false)
        {
            CurrentServer = currentServer;
            _log = log;
            _eventAggregator = eventAggregator;
            _console = console;

            IsBusy = true;

            BanControl = new ContentControl();
            OnlinePlayersControl = new ContentControl();
            PlayersControl = new ContentControl();
            ChatControl = new ContentControl();
            AdminsControl = new ContentControl();
            SteamControl = new ContentControl();
            ManageServerControl = new ContentControl();

            Task.Factory.StartNew(() => InitModel(ipService, container, console))
                .ContinueWith(t => IsBusy = false);
        }

        private void InitModel(IIpService ipService, IUnityContainer container, bool console)
        {
            var host = ipService.GetIpAddress(CurrentServer.Host);

            if (string.IsNullOrEmpty(host))
            {
                var message = $"Host is incorrect for server {CurrentServer.Name}";
                _log.Error(message);
                throw new Exception(message);
            }

            var steamQueryViewModel =
                container.Resolve<IServerMonitorSteamQueryViewModel>(new ParameterOverride("host", host),
                    new ParameterOverride("port", CurrentServer.Port));
            _eventAggregator.GetEvent<CreateViewEvent<IServerMonitorSteamQueryViewModel>>()
                  .Publish(new CreateViewModel<IServerMonitorSteamQueryViewModel>((ContentControl)SteamControl, steamQueryViewModel));


            //_beServer = container.Resolve<BEServer>(new ParameterOverride("host", host),
            //    new ParameterOverride("port", CurrentServer.Port),
            //    new ParameterOverride("password", CurrentServer.Password));


            _eventAggregator.GetEvent<RunServerEvent>().Publish(CurrentServer);

            //_beServer.ConnectHandler += BeServerConnectHandler;
            //_beServer.DisconnectHandler += BeServerDisconnectHandler;

            //if (!console)
            //{
            //    _beServer.RConAdminLog += (s, e) => _beServer.SendCommand(CommandType.Admins);
            //}

            //_beServer.PlayerLog += (s, e) => _beServer.SendCommand(CommandType.Players);

            //if (!console)
            //{
            //    _beServer.BanLog += (s, e) =>
            //    {
            //        _beServer.SendCommand(CommandType.Players);
            //        _beServer.SendCommand(CommandType.Bans);
            //    };
            //}

            // _beServer.ConnectingHandler += (s, e) => OnPropertyChanged(nameof(Connected));


            var playersViewModel =
                container.Resolve<IServerMonitorPlayerViewModel>(new ParameterOverride("serverInfo", CurrentServer));

            _eventAggregator.GetEvent<CreateViewEvent<IServerMonitorPlayerViewModel>>()
                   .Publish(new CreateViewModel<IServerMonitorPlayerViewModel>((ContentControl)OnlinePlayersControl, playersViewModel));


            if (!console)
            {

                var bansViewModel =
                    container.Resolve<IServerMonitorBansViewModel>(
                        new ParameterOverride("serverInfoId", CurrentServer.Id));


                _eventAggregator.GetEvent<CreateViewEvent<IServerMonitorBansViewModel>>()
                    .Publish(new CreateViewModel<IServerMonitorBansViewModel>((ContentControl)BanControl, bansViewModel));





                var adminsViewModel =
                   container.Resolve<IServerMonitorAdminsViewModel>(
                       new ParameterOverride("serverInfoId", CurrentServer.Id));


                _eventAggregator.GetEvent<CreateViewEvent<IServerMonitorAdminsViewModel>>()
                    .Publish(new CreateViewModel<IServerMonitorAdminsViewModel>((ContentControl)AdminsControl, adminsViewModel));


                //AdminsViewModel =
                //    container.Resolve<ServerMonitorAdminsViewModel>(new ParameterOverride("serverInfo", CurrentServer),
                //        new ParameterOverride("refreshCommand",
                //            new ActionCommand(() => _beServer.SendCommand(CommandType.Admins))));

                var manageServerViewModel =
                    container.Resolve<IServerMonitorManageServerViewModel>(
                        new ParameterOverride("serverId", CurrentServer.Id));

                _eventAggregator.GetEvent<CreateViewEvent<IServerMonitorManageServerViewModel>>()
                   .Publish(new CreateViewModel<IServerMonitorManageServerViewModel>((ContentControl)ManageServerControl, manageServerViewModel));


                var playerListModelView =
                    container.Resolve<IPlayerListModelView>(new ParameterOverride("serverId", CurrentServer.Id));

                _eventAggregator.GetEvent<CreateViewEvent<IPlayerListModelView>>()
                  .Publish(new CreateViewModel<IPlayerListModelView>((ContentControl)PlayersControl, playerListModelView));
            }

            var chatViewModel =
                container.Resolve<IServerMonitorChatViewModel>(new ParameterOverride("serverId", CurrentServer.Id));

            _eventAggregator.GetEvent<CreateViewEvent<IServerMonitorChatViewModel>>()
                  .Publish(new CreateViewModel<IServerMonitorChatViewModel>((ContentControl)ChatControl, chatViewModel));

            //OnPropertyChanged(nameof(AdminsViewModel));
            //OnPropertyChanged(nameof(ManageServerViewModel));
            //OnPropertyChanged(nameof(PlayerListModelView));
            //OnPropertyChanged(nameof(SteamQueryViewModel));

            OnPropertyChanged(nameof(BanControl));
            OnPropertyChanged(nameof(OnlinePlayersControl));
            OnPropertyChanged(nameof(PlayersControl));
            OnPropertyChanged(nameof(ChatControl));
            OnPropertyChanged(nameof(AdminsControl));
            OnPropertyChanged(nameof(SteamControl));
            OnPropertyChanged(nameof(ManageServerControl));
            //Connect();
        }

        public ServerInfo CurrentServer { get; }

        public bool Connected => false;


        private void BeServerDisconnectHandler(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(Connected));
        }

        private void BeServerConnectHandler(object sender, EventArgs e)
        {
            SendCommand(CommandType.Players);
            SendCommand(CommandType.Admins);
            SendCommand(CommandType.Missions);
            SendCommand(CommandType.Bans);

            OnPropertyChanged(nameof(Connected));
        }

        private void SendCommand(CommandType commandType, string parameters = null)
        {
            _eventAggregator.GetEvent<BEMessageEvent<BECommand>>()
                .Publish(new BECommand(CurrentServer.Id, commandType, parameters));
        }

        //public void Connect()
        //{
        //    try
        //    {
        //        if (!_beServer.Connected)
        //            _beServer.Connect();
        //    }
        //    catch (Exception ex)
        //    {
        //        _log.Error(ex);
        //    }
        //}

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();
            _eventAggregator.GetEvent<CloseServerEvent>()
                .Publish(CurrentServer);
            //_beServer?.Disconnect();
            //_beServer?.Dispose();
            //_beServer = null;
        }

        #region ViewModels


        public object ChatControl { get; set; }
        public object BanControl { get; set; }
        public object AdminsControl { get; set; }
        public object OnlinePlayersControl { get; set; }
        public object PlayersControl { get; set; }
        public object SteamControl { get; set; }
        public object ManageServerControl { get; set; }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    OnPropertyChanged(nameof(IsBusy));
                }
            }
        }

        #endregion
    }
}