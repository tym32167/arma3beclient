using Arma3BE.Client.Infrastructure.Contracts;
using Arma3BE.Client.Infrastructure.Events;
using Arma3BE.Client.Infrastructure.Events.BE;
using Arma3BE.Server.Models;
using Arma3BEClient.Libs.Repositories;
using Prism.Events;
using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

// ReSharper disable MemberCanBePrivate.Global

namespace Arma3BE.Client.Modules.MainModule.ViewModel
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ServerMonitorModel : DisposableViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IServerInfoRepository _infoRepository;
        private bool _connected;

        public ServerMonitorModel(ServerInfoDto currentServer,
            IEventAggregator eventAggregator, IServerInfoRepository infoRepository)
        {
            CurrentServer = currentServer;
            _eventAggregator = eventAggregator;
            _infoRepository = infoRepository;

            _eventAggregator.GetEvent<ConnectServerEvent>().Subscribe(BeServerConnectHandler, ThreadOption.UIThread);
            _eventAggregator.GetEvent<DisConnectServerEvent>().Subscribe(BeServerDisconnectHandler, ThreadOption.UIThread);
            _eventAggregator.GetEvent<RunServerEvent>().Publish(CurrentServer);

            _eventAggregator.GetEvent<BEMessageEvent<BEItemsMessage<Player>>>().Subscribe(PlayersMessage);
        }

        public ServerInfoDto CurrentServer { get; }

        private void PlayersMessage(BEItemsMessage<Player> items)
        {
            if (items.ServerId != CurrentServer.Id) return;
            Count = items?.Items?.Count() ?? 0;
        }

        public bool Connected
        {
            // ReSharper disable once UnusedMember.Global
            get { return _connected; }
            set
            {
                _connected = value;
                RaisePropertyChanged();
            }
        }

        private int _count;
        private int Count
        {
            get { return _count; }
            set
            {
                _count = value;
                RaisePropertyChanged(nameof(Title));
            }
        }

        public string Title
        {
            get => $"{CurrentServer.Name} ({Count})";
            set
            {

            }
        } 

        private void BeServerDisconnectHandler(ServerInfoDto info)
        {
            if (info.Id != CurrentServer.Id) return;
            Connected = false;
        }

        private void BeServerConnectHandler(ServerInfoDto info)
        {
            if (info.Id != CurrentServer.Id) return;
            Connected = true;
        }



        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();
            _eventAggregator.GetEvent<CloseServerEvent>()
                .Publish(CurrentServer);
        }

        public async Task SetActive(Guid serverId, bool active = false)
        {
            await _infoRepository.SetServerInfoActiveAsync(serverId, active);
            _eventAggregator.GetEvent<BEServersChangedEvent>().Publish(null);
        }

        public async Task CloseServerAsync()
        {
            // ReSharper disable once RedundantArgumentDefaultValue
            await SetActive(CurrentServer.Id, false);
        }

        public async Task OpenServerAsync()
        {
            await SetActive(CurrentServer.Id, true);
        }
    }
}