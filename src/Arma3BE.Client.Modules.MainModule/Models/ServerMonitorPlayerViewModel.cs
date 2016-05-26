using Arma3BE.Client.Infrastructure;
using Arma3BE.Client.Infrastructure.Commands;
using Arma3BE.Client.Infrastructure.Helpers;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Client.Modules.MainModule.Dialogs;
using Arma3BE.Client.Modules.MainModule.Helpers;
using Arma3BE.Server;
using Arma3BE.Server.Abstract;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Libs.ModelCompact;
using Prism.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Player = Arma3BE.Server.Models.Player;

namespace Arma3BE.Client.Modules.MainModule.Models
{
    public class ServerMonitorPlayerViewModel : ServerMonitorBaseViewModel<Player, Helpers.Views.PlayerView>
    {
        private readonly IBEServer _beServer;
        private readonly IBanService _banService;
        private readonly IPlayerViewService _playerViewService;
        internal readonly PlayerHelper PlayerHelper;

        public ServerMonitorPlayerViewModel(ILog log, ServerInfo serverInfo, IBEServer beServer, IBanHelper banHelper, IBanService banService, IPlayerViewService playerViewService)
            : base(new ActionCommand(() => beServer.SendCommand(CommandType.Players)))
        {
            _beServer = beServer;
            _banService = banService;
            _playerViewService = playerViewService;
            PlayerHelper = new PlayerHelper(log, serverInfo.Id, beServer, banHelper);

            KickUserCommand = new DelegateCommand(ShowKickDialog, CanShowDialog);
            BanUserCommand = new DelegateCommand(ShowBanDialog, CanShowDialog);
            PlayerInfoCommand = new DelegateCommand(PlayerInfoDialog, CanShowDialog);

            beServer.PlayerHandler += (s, e) => base.SetData(e.Data);
            beServer.AdminHandler += (s, e) => { _admins = e.Data ?? new Arma3BE.Server.Models.Admin[0]; };
        }

        private void ShowKickDialog()
        {
            var local = SelectedItem;
            if (local != null)
            {
                _banService.ShowKickDialog(_beServer, local.Num, local.Guid, local.Name);
            }
        }

        private void ShowBanDialog()
        {
            var local = SelectedItem;
            if (local != null)
            {
                _banService.ShowBanDialog(_beServer, local.Guid, true, local.Name, local.Num.ToString());
            }
        }

        private void PlayerInfoDialog()
        {
            var local = SelectedItem;
            if (local != null)
            {
                _playerViewService.ShowDialog(local.Guid);
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
            PlayerHelper.RegisterPlayers(enumerable);
            var view = PlayerHelper.GetPlayerView(enumerable).ToArray();
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