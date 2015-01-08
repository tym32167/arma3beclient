using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Arma3BEClient.Commands;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Helpers;
using Arma3BEClient.Lib.Context;
using Arma3BEClient.Lib.ModelCompact;
using Arma3BEClient.Lib.Tools;
using Arma3BEClient.Updater;
using Player = Arma3BEClient.Updater.Models.Player;

namespace Arma3BEClient.Models
{
    public class ServerMonitorPlayerViewModel : ServerMonitorBaseViewModel<Player, Helpers.Views.PlayerView>
    {
        private readonly ILog _log;
        private readonly UpdateClient _updateClient;
        internal readonly PlayerHelper _playerHelper;
        private PlayerView _selectedPlayer;

        public ServerMonitorPlayerViewModel(ILog log, ServerInfo serverInfo, UpdateClient updateClient )
            : base(new ActionCommand(() => updateClient.SendCommandAsync(UpdateClient.CommandType.Players)))
        {
            _log = log;
            _updateClient = updateClient;
            _playerHelper = new PlayerHelper(_log, serverInfo.Id, _updateClient);
        }

        protected override IEnumerable<Helpers.Views.PlayerView> RegisterData(IEnumerable<Player> initialData)
        {
            var enumerable = initialData as IList<Player> ?? initialData.ToList();
            _playerHelper.RegisterPlayers(enumerable);
            return _playerHelper.GetPlayerView(enumerable);
        }

        public ICommand KickUserCommand { get; set; }
    }
}