using Arma3BE.Client.Infrastructure.Contracts;
using Arma3BE.Client.Infrastructure.Events;
using Arma3BE.Client.Infrastructure.Events.BE;
using Arma3BE.Server;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Libs.ModelCompact;
using Arma3BEClient.Libs.Repositories;
using Prism.Events;
using System;
using System.Threading.Tasks;

namespace Arma3BE.Client.Modules.MainModule.ViewModel
{
    public class ServerMonitorModel : DisposableViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ILog _log;
        private bool _connected;
        private bool _isBusy;

        public ServerMonitorModel(ServerInfo currentServer, ILog log,
            IEventAggregator eventAggregator)
        {
            CurrentServer = currentServer;
            _log = log;
            _eventAggregator = eventAggregator;

            IsBusy = true;

            Task.Factory.StartNew(InitModel)
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


        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Title { get { return CurrentServer.Name; } }

        private void InitModel()
        {
            _eventAggregator.GetEvent<ConnectServerEvent>().Subscribe(BeServerConnectHandler);
            _eventAggregator.GetEvent<DisConnectServerEvent>().Subscribe(BeServerDisconnectHandler);
            _eventAggregator.GetEvent<RunServerEvent>().Publish(CurrentServer);
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

        public void SetActive(Guid serverId, bool active = false)
        {
            using (var repo = new ServerInfoRepository())
            {
                repo.SetServerInfoActive(serverId, active);
            }

            _eventAggregator.GetEvent<BEServersChangedEvent>().Publish(null);
        }

        public void CloseServer()
        {
            SetActive(CurrentServer.Id, false);
        }

        public void OpenServer()
        {
            SetActive(CurrentServer.Id, true);
        }
    }
}