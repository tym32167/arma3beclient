using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Arma3BE.Client.Infrastructure.Commands;
using Arma3BE.Client.Modules.MainModule.Helpers;
using Arma3BE.Server;
using Arma3BE.Server.Abstract;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Libs.ModelCompact;
using Player = Arma3BE.Server.Models.Player;

namespace Arma3BE.Client.Modules.MainModule.Models
{
    public class ServerMonitorPlayerViewModel : ServerMonitorBaseViewModel<Player, Helpers.Views.PlayerView>
    {
        internal readonly PlayerHelper PlayerHelper;

        public ServerMonitorPlayerViewModel(ILog log, ServerInfo serverInfo, IBEServer beServer)
            : base(new ActionCommand(() => beServer.SendCommand(CommandType.Players)))
        {
            PlayerHelper = new PlayerHelper(log, serverInfo.Id, beServer);
            beServer.PlayerHandler += (s, e) => base.SetData(e.Data);
            beServer.AdminHandler += (s, e) => { _admins = e.Data ?? new Arma3BE.Server.Models.Admin[0]; };
        }

        private IEnumerable<Arma3BE.Server.Models.Admin> _admins = new List<Arma3BE.Server.Models.Admin>();

        public ICommand KickUserCommand { get; set; }

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