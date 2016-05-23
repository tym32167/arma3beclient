using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Arma3BE.Client.Infrastructure.Commands;
using Arma3BE.Client.Modules.MainModule.Helpers;
using Arma3BE.Client.Modules.MainModule.Models;
using Arma3BE.Server.Abstract;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Libs.Repositories;
using GalaSoft.MvvmLight;

namespace Arma3BE.Client.Modules.MainModule.ViewModel
{
    public class PlayerListModelView : ViewModelBase
    {
        private int _playerCount;
        internal readonly PlayerHelper PlayerHelper;
        private ICommand _refreshCommand;

        public PlayerListModelView(ILog log, IBEServer beServer, Guid serverId)
        {
            PlayerHelper = new PlayerHelper(log, serverId, beServer);
            SelectedOptions = "Name,IP,Guid,Comment";
        }

        public string Filter { get; set; }

        public int PlayerCount
        {
            get { return _playerCount; }
            set
            {
                _playerCount = value;
                RaisePropertyChanged();
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
                {
                    result = repo.GetPlayers(x =>
                        (searchGuid && x.GUID == Filter)
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
                }
                else
                {
                    result = repo.GetAllPlayers();
                }

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

            RaisePropertyChanged(nameof(Players));
        }
    }
}