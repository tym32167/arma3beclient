using Arma3BE.Client.Infrastructure.Commands;
using Arma3BE.Client.Infrastructure.Events;
using Arma3BE.Client.Infrastructure.Events.Models;
using Arma3BE.Client.Infrastructure.Extensions;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Client.Modules.PlayersModule.Models;
using Arma3BEClient.Libs.Repositories;
using Prism.Commands;
using Prism.Events;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ExplicitCallerInfoArgument
// ReSharper disable UnusedMember.Global

namespace Arma3BE.Client.Modules.PlayersModule.ViewModel
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PlayerListModelView : ViewModelBase, ITitledItem
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IPlayerRepository _playerRepository;
        private int _playerCount;
        private ICommand _refreshCommand;

        public PlayerListModelView(IEventAggregator eventAggregator, IPlayerRepository playerRepository)
        {
            _eventAggregator = eventAggregator;
            _playerRepository = playerRepository;
            SelectedOptions = "Name,IP,Guid,Comment";

            BanCommand = new ActionCommand(ShowBan);
            PlayerInfoCommand = new DelegateCommand(PlayerInfoDialog, CanShowDialog);
        }

        public PlayerView SelectedPlayer { get; set; }

        public ICommand BanCommand { get; set; }
        public ICommand PlayerInfoCommand { get; set; }


        public string Filter { get; set; }

        public int PlayerCount
        {
            get { return _playerCount; }
            set
            {
                _playerCount = value;
                OnPropertyChanged();
            }
        }

        public ICommand RefreshCommand => _refreshCommand ?? (_refreshCommand = new ActionCommand(async () => await Refresh()));

        public List<PlayerView> Players { get; private set; } = new List<PlayerView>();

        public string SelectedOptions { get; set; }

        public IEnumerable<string> SearchOptions => new[]
        {
            "Name",
            "Last Names",
            "IP",
            "Guid",
            "Notes",
            "Comment",
            nameof(PlayerView.SteamId)
        };

        public static string StaticTitle = "Players";
        public string Title => StaticTitle;

        private void ShowBan()
        {
            var local = SelectedPlayer;
            if (local != null)
                _eventAggregator.GetEvent<BanUserEvent>()
                    .Publish(new BanUserModel(null, local.Guid, false, local.Name, null));
        }

        private bool CanShowDialog()
        {
            return SelectedPlayer != null;
        }

        private void PlayerInfoDialog()
        {
            var local = SelectedPlayer;
            if (local != null)
                _eventAggregator.GetEvent<ShowUserInfoEvent>().Publish(new ShowUserModel(local.Guid));
        }

        public async Task Refresh()
        {

            var opts = SelectedOptions.Split(',');

            var searchName = opts.Contains(nameof(PlayerView.Name));
            var searchLastNames = opts.Contains("Last Names");
            var searchIP = opts.Contains("IP");
            var searchGuid = opts.Contains("Guid");
            var searchNotes = opts.Contains("Notes");
            var searchComment = opts.Contains(nameof(PlayerView.Comment));
            var searchSteamId = opts.Contains(nameof(PlayerView.SteamId));

            IEnumerable<PlayerDto> result;

            if (!string.IsNullOrEmpty(Filter))
                result = await _playerRepository.GetPlayersAsync(x =>
                    (searchGuid && (x.GUID == Filter))
                    ||
                    (searchComment && x.Comment.Contains(Filter))
                    ||
                    (searchName && x.Name.Contains(Filter))
                    ||
                    (searchNotes && x.Notes.Any(y => y.Text.Contains(Filter)))
                    ||
                    (searchIP && x.LastIp.Contains(Filter))
                    ||
                    (searchSteamId && x.SteamId.Contains(Filter))
                    ||
                    (searchLastNames && x.PlayerHistory.Any(y => y.Name.Contains(Filter))));
            else
                result = await _playerRepository.GetAllPlayersAsync();

            var r = result.Select(x => new PlayerView
            {
                Id = x.Id,
                Comment = x.Comment,
                Guid = x.GUID,
                Name = x.Name,
                LastIp = x.LastIp,
                LastSeen = x.LastSeen,
                SteamId = x.SteamId
            }).OrderBy(x => x.Name).ToList();

            foreach (var playerView in r)
            {
                playerView.LastSeen = playerView.LastSeen.UtcToLocalFromSettings();
            }

            PlayerCount = r.Count;

            Players = r;


            OnPropertyChanged(nameof(Players));
        }
    }
}