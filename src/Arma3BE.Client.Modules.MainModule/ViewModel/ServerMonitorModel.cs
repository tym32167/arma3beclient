using System.Threading.Tasks;
using System.Windows.Controls;
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
using Prism.Regions;

namespace Arma3BE.Client.Modules.MainModule.ViewModel
{
    public class ServerMonitorModel : DisposableViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ILog _log;
        private bool _connected;
        private bool _isBusy;

        public ServerMonitorModel(ServerInfo currentServer, ILog log, IUnityContainer container,
            IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            CurrentServer = currentServer;
            _log = log;
            _eventAggregator = eventAggregator;

            IsBusy = true;

            //BanControl = new ContentControl();
            //OnlinePlayersControl = new ContentControl();
            //PlayersControl = new ContentControl();
            //ChatControl = new ContentControl();
            //AdminsControl = new ContentControl();
            //SteamControl = new ContentControl();
            //ManageServerControl = new ContentControl();

            Task.Factory.StartNew(() => InitModel(container))
                .ContinueWith(t => IsBusy = false);
        }

        public ServerInfo CurrentServer { get; }

        public bool Connected
        {
            get { return _connected; }
            set
            {
                _connected = value;
                OnPropertyChanged();
            }
        }

        private void InitModel(IUnityContainer container)
        {
            _eventAggregator.GetEvent<ConnectServerEvent>().Subscribe(BeServerConnectHandler);
            _eventAggregator.GetEvent<DisConnectServerEvent>().Subscribe(BeServerDisconnectHandler);

            _eventAggregator.GetEvent<RunServerEvent>().Publish(CurrentServer);

            //var steamQueryViewModel =
            //    container.Resolve<IServerMonitorSteamQueryViewModel>(new ParameterOverride("serverInfo", CurrentServer));
            //_eventAggregator.GetEvent<CreateViewEvent<IServerMonitorSteamQueryViewModel>>()
            //    .Publish(new CreateViewModel<IServerMonitorSteamQueryViewModel>((ContentControl)SteamControl,
            //        steamQueryViewModel));

            //var playersViewModel =
            //    container.Resolve<IServerMonitorPlayerViewModel>(new ParameterOverride("serverInfo", CurrentServer));

            //_eventAggregator.GetEvent<CreateViewEvent<IServerMonitorPlayerViewModel>>()
            //    .Publish(new CreateViewModel<IServerMonitorPlayerViewModel>((ContentControl)OnlinePlayersControl,
            //        playersViewModel));

            //var bansViewModel =
            //    container.Resolve<IServerMonitorBansViewModel>(
            //        new ParameterOverride("serverInfo", CurrentServer));

            //_eventAggregator.GetEvent<CreateViewEvent<IServerMonitorBansViewModel>>()
            //    .Publish(new CreateViewModel<IServerMonitorBansViewModel>((ContentControl)BanControl, bansViewModel));


            //var adminsViewModel =
            //    container.Resolve<IServerMonitorAdminsViewModel>(
            //        new ParameterOverride("serverInfo", CurrentServer));

            //_eventAggregator.GetEvent<CreateViewEvent<IServerMonitorAdminsViewModel>>()
            //    .Publish(new CreateViewModel<IServerMonitorAdminsViewModel>((ContentControl)AdminsControl,
            //        adminsViewModel));


            //var manageServerViewModel =
            //    container.Resolve<IServerMonitorManageServerViewModel>(
            //        new ParameterOverride("serverInfo", CurrentServer));

            //_eventAggregator.GetEvent<CreateViewEvent<IServerMonitorManageServerViewModel>>()
            //    .Publish(
            //        new CreateViewModel<IServerMonitorManageServerViewModel>((ContentControl)ManageServerControl,
            //            manageServerViewModel));

            //var playerListModelView =
            //    container.Resolve<IPlayerListModelView>(new ParameterOverride("serverInfo", CurrentServer));

            //_eventAggregator.GetEvent<CreateViewEvent<IPlayerListModelView>>()
            //    .Publish(new CreateViewModel<IPlayerListModelView>((ContentControl)PlayersControl,
            //        playerListModelView));

            //var chatViewModel =
            //    container.Resolve<IServerMonitorChatViewModel>(new ParameterOverride("serverInfo", CurrentServer));

            //_eventAggregator.GetEvent<CreateViewEvent<IServerMonitorChatViewModel>>()
            //    .Publish(new CreateViewModel<IServerMonitorChatViewModel>((ContentControl)ChatControl, chatViewModel));

            //OnPropertyChanged(nameof(BanControl));
            //OnPropertyChanged(nameof(OnlinePlayersControl));
            //OnPropertyChanged(nameof(PlayersControl));
            //OnPropertyChanged(nameof(ChatControl));
            //OnPropertyChanged(nameof(AdminsControl));
            //OnPropertyChanged(nameof(SteamControl));
            //OnPropertyChanged(nameof(ManageServerControl));
        }

        private void BeServerDisconnectHandler(ServerInfo info)
        {
            if (info.Id != CurrentServer.Id) return;
            Connected = false;
        }

        private void BeServerConnectHandler(ServerInfo info)
        {
            if (info.Id != CurrentServer.Id) return;
            SendCommand(CommandType.Players);
            SendCommand(CommandType.Admins);
            SendCommand(CommandType.Missions);
            SendCommand(CommandType.Bans);

            Connected = true;
        }

        private void SendCommand(CommandType commandType, string parameters = null)
        {
            _eventAggregator.GetEvent<BEMessageEvent<BECommand>>()
                .Publish(new BECommand(CurrentServer.Id, commandType, parameters));
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();
            _eventAggregator.GetEvent<CloseServerEvent>()
                .Publish(CurrentServer);
        }

        #region ViewModels

        //public object ChatControl { get; }
        //public object BanControl { get; }
        //public object AdminsControl { get; }
        //public object OnlinePlayersControl { get; }
        //public object PlayersControl { get; }
        //public object SteamControl { get; }
        //public object ManageServerControl { get; }

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