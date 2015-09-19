using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Arma3BE.Server;
using Arma3BEClient.Commands;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Helpers;
using Arma3BEClient.Libs.ModelCompact;
using Player = Arma3BE.Server.Models.Player;

namespace Arma3BEClient.Models
{
    public class ServerMonitorPlayerViewModel : ServerMonitorBaseViewModel<Player, Helpers.Views.PlayerView>
    {
        private readonly ILog _log;
        internal readonly PlayerHelper _playerHelper;
        private readonly UpdateClient _updateClient;
        private PlayerView _selectedPlayer;

        public ServerMonitorPlayerViewModel(ILog log, ServerInfo serverInfo, UpdateClient updateClient)
            : base(new ActionCommand(() => updateClient.SendCommandAsync(UpdateClient.CommandType.Players)))
        {
            _log = log;
            _updateClient = updateClient;
            _playerHelper = new PlayerHelper(_log, serverInfo.Id, _updateClient);
        }

        public ICommand KickUserCommand { get; set; }

        protected override IEnumerable<Helpers.Views.PlayerView> RegisterData(IEnumerable<Player> initialData)
        {
            var enumerable = initialData as IList<Player> ?? initialData.ToList();
            _playerHelper.RegisterPlayers(enumerable);
            return _playerHelper.GetPlayerView(enumerable);
        }
    }
}