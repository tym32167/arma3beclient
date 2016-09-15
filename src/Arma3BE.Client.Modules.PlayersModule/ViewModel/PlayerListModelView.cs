using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Arma3BE.Client.Infrastructure.Commands;
using Arma3BE.Client.Infrastructure.Events;
using Arma3BE.Client.Infrastructure.Events.Models;
using Arma3BE.Client.Infrastructure.Models;
using Arma3BE.Client.Modules.PlayersModule.Models;
using Arma3BEClient.Libs.Repositories;
using Prism.Commands;
using Prism.Events;

namespace Arma3BE.Client.Modules.PlayersModule.ViewModel
{
    public class PlayerListModelView : ViewModelBase, ITitledItem
    {
        private readonly IEventAggregator _eventAggregator;
        private int _playerCount;
        private ICommand _refreshCommand;

        public PlayerListModelView(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
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

        public ICommand RefreshCommand
        {
            get { return _refreshCommand ?? (_refreshCommand = new ActionCommand(Refresh)); }
        }

        public List<PlayerView> Players { get; private set; } = new List<PlayerView>();

        public string SelectedOptions { get; set; }

        public IEnumerable<string> SearchOptions
        {
            get
            {
                return new[]
                {
                    "Name",
                    "Last Names",
                    "IP",
                    "Guid",
                    "Notes",
                    "Comment"
                };
            }
        }

        public string Title
        {
            get { return "Players"; }
        }

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

        public void Refresh()
        {
            using (var repo = PlayerRepositoryFactory.Create())
            {
                var opts = SelectedOptions.Split(',');

                var searchName = opts.Contains("Name");
                var searchLastNames = opts.Contains("Last Names");
                var searchIP = opts.Contains("IP");
                var searchGuid = opts.Contains("Guid");
                var searchNotes = opts.Contains("Notes");
                var searchComment = opts.Contains("Comment");

                IEnumerable<PlayerDto> result;

                if (!string.IsNullOrEmpty(Filter))
                    result = repo.GetPlayers(x =>
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
                        (searchLastNames && x.PlayerHistory.Any(y => y.Name.Contains(Filter))));
                else
                    result = repo.GetAllPlayers();

                var r = result.Select(x => new PlayerView
                {
                    Id = x.Id,
                    Comment = x.Comment,
                    Guid = x.GUID,
                    Name = x.Name,
                    LastIp = x.LastIp,
                    LastSeen = x.LastSeen
                }).OrderBy(x => x.Name).ToList();

                PlayerCount = r.Count;

                Players = r;
            }

            OnPropertyChanged(nameof(Players));
        }
    }
}