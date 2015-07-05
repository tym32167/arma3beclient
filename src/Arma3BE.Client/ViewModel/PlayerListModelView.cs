using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Arma3BEClient.Commands;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Helpers;
using Arma3BEClient.Libs.Context;
using Arma3BEClient.Models;
using Arma3BEClient.Updater;
using GalaSoft.MvvmLight;

namespace Arma3BEClient.ViewModel
{
    public class PlayerListModelView : ViewModelBase
    {
        private readonly ILog _log;
        private readonly Guid _serverId;
        private readonly UpdateClient _updateClient;
        private int _playerCount;
        internal PlayerHelper _playerHelper;
        private List<PlayerView> _players = new List<PlayerView>();
        private ICommand _refreshCommand;

        public PlayerListModelView(ILog log, UpdateClient updateClient, Guid serverId)
        {
            _log = log;
            _updateClient = updateClient;
            _serverId = serverId;

            _playerHelper = new PlayerHelper(_log, serverId, _updateClient);
            SelectedOptions = "Name,IP,Guid,Comment";
        }

        public string Filter { get; set; }

        public int PlayerCount
        {
            get { return _playerCount; }
            set
            {
                _playerCount = value;
                RaisePropertyChanged("PlayerCount");
            }
        }

        public ICommand RefreshCommand
        {
            get { return _refreshCommand ?? (_refreshCommand = new ActionCommand(Refresh)); }
        }

        public List<PlayerView> Players
        {
            get { return _players; }
        }

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
            using (var context = new Arma3BeClientContext())
            {
                var opts = SelectedOptions.Split(',');

                var searchName = opts.Contains("Name");
                var searchLastNames = opts.Contains("Last Names");
                var searchIP = opts.Contains("IP");
                var searchGuid = opts.Contains("Guid");
                var searchNotes = opts.Contains("Notes");
                var searchComment = opts.Contains("Comment");

                var result = context.Player.AsQueryable();
                if (!string.IsNullOrEmpty(Filter))
                {
                    result = result.Where(x =>
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
                        (searchLastNames && x.PlayerHistory.Any(y => y.Name.Contains(Filter)))
                        );
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

                _players = r;
            }

            RaisePropertyChanged("Players");
        }
    }
}