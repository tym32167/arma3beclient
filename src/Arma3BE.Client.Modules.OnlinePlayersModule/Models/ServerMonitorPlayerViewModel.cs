using Arma3BE.Client.Infrastructure.Commands;
using Arma3BE.Client.Infrastructure.Events;
using Arma3BE.Client.Infrastructure.Events.BE;
using Arma3BE.Client.Infrastructure.Events.Models;
using Arma3BE.Client.Infrastructure.Helpers;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Client.Modules.OnlinePlayersModule.Helpers;
using Arma3BE.Server;
using Arma3BEClient.Libs.Repositories;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Admin = Arma3BE.Server.Models.Admin;
using Player = Arma3BE.Server.Models.Player;
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable RedundantNameQualifier

namespace Arma3BE.Client.Modules.OnlinePlayersModule.Models
{
    public class ServerMonitorPlayerViewModel : ServerMonitorBaseViewModel<Player, Helpers.Views.PlayerView>
    {
        private readonly ServerInfoDto _serverInfo;
        private readonly IEventAggregator _eventAggregator;
        private readonly PlayerHelper _playerHelper;

        public ServerMonitorPlayerViewModel(ServerInfoDto serverInfo,
            IBanHelper banHelper, IEventAggregator eventAggregator, IPlayerRepository playerRepository, ReasonRepository reasonRepository)
            : base(
                new ActionCommand(() => SendCommand(eventAggregator, serverInfo.Id, CommandType.Players)),
                new PlayerViewComparer())
        {
            _serverInfo = serverInfo;
            _eventAggregator = eventAggregator;
            _playerHelper = new PlayerHelper(serverInfo.Id, banHelper, playerRepository, reasonRepository);

            KickUserCommand = new DelegateCommand(ShowKickDialog, CanShowDialog);
            BanUserCommand = new DelegateCommand(ShowBanDialog, CanShowDialog);
            PlayerInfoCommand = new DelegateCommand(PlayerInfoDialog, CanShowDialog);


            PropertyChanged += ServerMonitorPlayerViewModel_PropertyChanged;

            _eventAggregator.GetEvent<BEMessageEvent<BEItemsMessage<Player>>>().Subscribe(e =>
            {
                if (e.ServerId == serverInfo.Id)
                {
                    SetDataAsync(e.Items);
                    WaitingForEvent = false;
                }
            });

            _eventAggregator.GetEvent<BEMessageEvent<BEItemsMessage<Admin>>>().Subscribe(e =>
            {
                if (e.ServerId == serverInfo.Id)
                {
                    _admins = e.Items ?? new Admin[0];
                }
            });
        }

        private void ServerMonitorPlayerViewModel_PropertyChanged(object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedItem) || e.PropertyName == nameof(Data))
            {
                KickUserCommand?.RaiseCanExecuteChanged();
                BanUserCommand?.RaiseCanExecuteChanged();
                PlayerInfoCommand?.RaiseCanExecuteChanged();
            }
        }

        public string Title => "Session";

        private static void SendCommand(IEventAggregator eventAggregator, Guid serverId, CommandType commandType,
            string parameters = null)
        {
            eventAggregator.GetEvent<BEMessageEvent<BECommand>>()
                .Publish(new BECommand(serverId, commandType, parameters));
        }

        private void ShowKickDialog()
        {
            var local = SelectedItem;
            if (local != null)
            {
                _eventAggregator.GetEvent<KickUserEvent>()
                    .Publish(new KickUserModel(_serverInfo.Id, local.Guid, local.Name, local.Num));
            }
        }

        private void ShowBanDialog()
        {
            var local = SelectedItem;
            if (local != null)
            {
                _eventAggregator.GetEvent<BanUserEvent>()
                    .Publish(new BanUserModel(_serverInfo.Id, local.Guid, true, local.Name, local.Num.ToString()));
            }
        }

        private void PlayerInfoDialog()
        {
            var local = SelectedItem;
            if (local != null)
            {
                _eventAggregator.GetEvent<ShowUserInfoEvent>().Publish(new ShowUserModel(local.Guid));
            }
        }

        private bool CanShowDialog()
        {
            return SelectedItem != null;
        }

        private IEnumerable<Admin> _admins = new List<Admin>();

        public DelegateCommand KickUserCommand { get; }
        public DelegateCommand BanUserCommand { get; }
        public DelegateCommand PlayerInfoCommand { get; }

        protected override async Task<IEnumerable<Helpers.Views.PlayerView>> RegisterDataAsync(IEnumerable<Player> initialData)
        {
            var enumerable = initialData as IList<Player> ?? initialData.ToList();
            _playerHelper.RegisterPlayers(enumerable);
            var view = (await _playerHelper.GetPlayerViewAsync(enumerable)).ToArray();
            var admins = _admins ?? new Admin[0];
            var adminsIps = new HashSet<string>(admins.Select(x => x.IP.ToLower()).Distinct());
            foreach (var playerView in view)
            {
                playerView.CanBeAdmin = adminsIps.Contains(playerView.IP.ToLower());
            }
            return view;
        }


        private class PlayerViewComparer : IEqualityComparer<Helpers.Views.PlayerView>
        {
            public bool Equals(Helpers.Views.PlayerView x, Helpers.Views.PlayerView y)
            {
                return x.Id == y.Id && x.Guid == y.Guid;
            }

            public int GetHashCode(Helpers.Views.PlayerView obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}