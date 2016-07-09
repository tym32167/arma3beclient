using Arma3BE.Server;
using Arma3BEClient.Commands;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Helpers;
using Arma3BEClient.Libs.ModelCompact;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Player = Arma3BE.Server.Models.Player;

namespace Arma3BEClient.Models
{
    public class ServerMonitorPlayerViewModel : ServerMonitorBaseViewModel<Player, Helpers.Views.PlayerView>
    {
        private readonly IBEServer _beServer;
        private readonly ILog _log;
        internal readonly PlayerHelper _playerHelper;
        private PlayerView _selectedPlayer;

        public ServerMonitorPlayerViewModel(ILog log, ServerInfo serverInfo, IBEServer beServer)
            : base(new ActionCommand(() => beServer.SendCommand(CommandType.Players)), new PlayerViewComperer())
        {
            _log = log;
            _beServer = beServer;
            _playerHelper = new PlayerHelper(_log, serverInfo.Id, _beServer);

            _beServer.PlayerHandler += (s, e) => SetData(e.Data);

            _beServer.AdminHandler += (s, e) => { _admins = e.Data ?? new Arma3BE.Server.Models.Admin[0]; };
        }

        private IEnumerable<Arma3BE.Server.Models.Admin> _admins = new List<Arma3BE.Server.Models.Admin>();

        public ICommand KickUserCommand { get; set; }

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

        private class PlayerViewComperer : IEqualityComparer<Helpers.Views.PlayerView>
        {
            public bool Equals(Helpers.Views.PlayerView x, Helpers.Views.PlayerView y)
            {
                return x.Id == y.Id && x.Guid == y.Guid && x.State == y.State;
            }

            public int GetHashCode(Helpers.Views.PlayerView obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}