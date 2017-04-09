using Arma3BE.Client.Infrastructure.Helpers;
using Arma3BE.Client.Modules.OnlinePlayersModule.Helpers.Views;
using Arma3BEClient.Libs.ModelCompact;
using Arma3BEClient.Libs.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Arma3BEClient.Common.Extensions;
using Arma3BEClient.Common.Logging;
using Player = Arma3BE.Server.Models.Player;

namespace Arma3BE.Client.Modules.OnlinePlayersModule.Helpers
{
    public class PlayerHelper : StateHelper<Player>
    {
        private readonly ILog _log = LogFactory.Create(new StackTrace().GetFrame(0).GetMethod().DeclaringType);
        private readonly IBanHelper _banHelper;
        private readonly IPlayerRepository _playerRepository;
        private readonly Guid _serverId;

        private readonly Regex _nameRegex = new Regex("[A-Za-zА-Яа-я0-9]+",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        public PlayerHelper(Guid serverId, IBanHelper banHelper, IPlayerRepository playerRepository)
        {
            _serverId = serverId;
            _banHelper = banHelper;
            _playerRepository = playerRepository;
        }

        public void RegisterPlayers(IEnumerable<Player> list)
        {
            Task.Run(() => RegisterPlayersInternal(list));
        }


        private IEnumerable<Player> _previousState = new Player[0];

        private bool RegisterPlayersInternal(IEnumerable<Player> list)
        {
            var players = list.ToList();

            if (!HaveChanges(players, x => x.Num))
                return false;

            var prevoius = new HashSet<string>(_previousState.Select(x => x.Guid).Distinct());
            _previousState = players;

            var guids = players.Select(x => x.Guid).ToList();

            var playersInDb = _playerRepository.GetPlayers(guids).ToArray();
            var dbGuids = playersInDb.Select(x => x.GUID).ToList();

            var historyToAdd = new List<PlayerHistory>();
            var playerToUpdate = new List<PlayerDto>();

            foreach (var player in playersInDb)
            {
                var p = players.FirstOrDefault(x => x.Guid == player.GUID);
                if (p != null)
                {
                    var needUpdate = false;
                    if ((player.Name != p.Name) || (player.LastIp != p.IP))
                    {
                        historyToAdd.Add(new PlayerHistory
                        {
                            IP = p.IP,
                            Name = p.Name,
                            PlayerId = player.Id,
                            ServerId = _serverId
                        });

                        player.Name = p.Name;
                        player.LastIp = p.IP;

                        needUpdate = true;
                    }
                    if (prevoius.Contains(player.GUID) == false)
                    {
                        player.LastSeen = DateTime.UtcNow;
                        needUpdate = true;
                    }

                    if (needUpdate)
                        playerToUpdate.Add(player);
                }
            }

            var newplayers = players.Where(x => !dbGuids.Contains(x.Guid)).ToList();

            if (newplayers.Any())
                foreach (var p in newplayers)
                {
                    var np = new Arma3BEClient.Libs.ModelCompact.Player
                    {
                        GUID = p.Guid,
                        Name = p.Name,
                        Id = Guid.NewGuid(),
                        LastIp = p.IP
                    };

                    playerToUpdate.Add(np);

                    historyToAdd.Add(new PlayerHistory
                    {
                        IP = np.LastIp,
                        Name = np.Name,
                        PlayerId = np.Id,
                        ServerId = _serverId
                    });
                }

            _playerRepository.AddOrUpdate(playerToUpdate);
            _playerRepository.AddHistory(historyToAdd);


            return true;
        }

        public IEnumerable<PlayerView> GetPlayerView(IEnumerable<Player> list)
        {
            using (_log.Time("GetPlayerView"))
            {

                var players = list.ToList();
                var guids = players.Select(x => x.Guid).ToList();

                var playersInDb = _playerRepository.GetPlayers(guids);

                var result = players.Select(x => new PlayerView
                {
                    Guid = x.Guid,
                    IP = x.IP,
                    Name = x.Name,
                    Num = x.Num,
                    Ping = x.Ping,
                    State = x.State,
                    Port = x.Port
                }).ToList();

                result.ForEach(x =>
                {
                    var p = playersInDb.FirstOrDefault(y => y.GUID == x.Guid);
                    if (p != null)
                    {
                        x.Id = p.Id;
                        x.Comment = p.Comment;
                        x.SteamId = p.SteamId;
                    }
                });

                var filterUsers = result.FirstOrDefault(x => !_nameRegex.IsMatch(x.Name));
                if (filterUsers != null)
                {
#pragma warning disable 4014
                    _banHelper.Kick(_serverId, filterUsers.Num, filterUsers.Guid, "bot: Fill Nickname");
#pragma warning restore 4014
                }

                var badNicknames = ConfigurationManager.AppSettings["Bad_Nicknames"];
                if (!string.IsNullOrEmpty(badNicknames))
                {
                    var names = badNicknames.ToLower().Split('|').Distinct().ToDictionary(x => x);


                    var bad =
                        result.FirstOrDefault(x => !string.IsNullOrEmpty(x.Name) && names.ContainsKey(x.Name.ToLower()));

                    if (bad != null)
#pragma warning disable 4014
                        _banHelper.Kick(_serverId, bad.Num, bad.Guid, "bot: Bad Nickname");
#pragma warning restore 4014
                }


                return result;
            }
        }
    }
}