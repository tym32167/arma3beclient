using Arma3BE.Client.Infrastructure.Commands;
using Arma3BE.Client.Infrastructure.Events;
using Arma3BE.Client.Infrastructure.Events.BE;
using Arma3BE.Client.Infrastructure.Events.Models;
using Arma3BE.Client.Infrastructure.Helpers;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Client.Modules.OnlinePlayersModule.Helpers;
using Arma3BE.Server;
using Arma3BE.Server.Abstract;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Libs.ModelCompact;
using Prism.Commands;
using Prism.Events;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Player = Arma3BE.Server.Models.Player;

namespace Arma3BE.Client.Modules.OnlinePlayersModule.Models
{
    public class ServerMonitorPlayerViewModel : ServerMonitorBaseViewModel<Player, Helpers.Views.PlayerView>, IServerMonitorPlayerViewModel
    {
        private readonly ServerInfo _serverInfo;
        private readonly IBEServer _beServer;
        private readonly IEventAggregator _eventAggregator;
        private readonly PlayerHelper _playerHelper;

        public ServerMonitorPlayerViewModel(ILog log, ServerInfo serverInfo,
            IBEServer beServer, IBanHelper banHelper, IEventAggregator eventAggregator)
            : base(new ActionCommand(() => beServer.SendCommand(CommandType.Players)))
        {
            _serverInfo = serverInfo;
            _beServer = beServer;
            _eventAggregator = eventAggregator;
            _playerHelper = new PlayerHelper(log, serverInfo.Id, banHelper);

            KickUserCommand = new DelegateCommand(ShowKickDialog, CanShowDialog);
            BanUserCommand = new DelegateCommand(ShowBanDialog, CanShowDialog);
            PlayerInfoCommand = new DelegateCommand(PlayerInfoDialog, CanShowDialog);

            //_beServer.PlayerHandler += (s, e) => base.SetData(e.Data);
            _beServer.AdminHandler += (s, e) => { _admins = e.Data ?? new Arma3BE.Server.Models.Admin[0]; };

            _eventAggregator.GetEvent<BEMessageEvent<BEItemsMessage<Player>>>().Subscribe(e =>
            {
                if (e.ServerId == serverInfo.Id)
                {
                    base.SetData(e.Items);
                }
            });
        }

        private void ShowKickDialog()
        {
            var local = SelectedItem;
            if (local != null)
            {
                _eventAggregator.GetEvent<KickUserEvent>().Publish(new KickUserModel(_serverInfo.Id, local.Guid, local.Name, local.Num));
            }
        }

        private void ShowBanDialog()
        {
            var local = SelectedItem;
            if (local != null)
            {
                _eventAggregator.GetEvent<BanUserEvent>().Publish(new BanUserModel(_beServer, local.Guid, true, local.Name, local.Num.ToString()));
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

        private IEnumerable<Arma3BE.Server.Models.Admin> _admins = new List<Arma3BE.Server.Models.Admin>();

        public ICommand KickUserCommand { get; set; }
        public ICommand BanUserCommand { get; set; }
        public ICommand PlayerInfoCommand { get; set; }

        protected override IEnumerable<Helpers.Views.PlayerView> RegisterData(IEnumerable<Player> initialData)
        {
            var enumerable = initialData as IList<Player> ?? initialData.ToList();
            _playerHelper.RegisterPlayers(enumerable);
            var view = _playerHelper.GetPlayerView(enumerable).ToArray();
            var admins = _admins ?? new Arma3BE.Server.Models.Admin[0];
            var adminsIps = new HashSet<string>(admins.Select(x => x.IP.ToLower()).Distinct());
            foreach (var playerView in view)
            {
                playerView.CanBeAdmin = adminsIps.Contains(playerView.IP.ToLower());
            }
            return view;
        }
    }
}