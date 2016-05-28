using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Arma3BE.Client.Infrastructure.Helpers;
using Arma3BE.Client.Modules.OnlinePlayersModule.Helpers.Views;
using Arma3BE.Server.Abstract;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Libs.ModelCompact;
using Arma3BEClient.Libs.Repositories;
using Player = Arma3BE.Server.Models.Player;

namespace Arma3BE.Client.Modules.OnlinePlayersModule.Helpers
{
    public class PlayerHelper : StateHelper<Player>
    {
        private readonly IBEServer _beServer;
        private readonly IBanHelper _banHelper;
        private readonly ILog _log;
        private readonly Guid _serverId;

        private readonly Regex NameRegex = new Regex("[A-Za-zА-Яа-я0-9]+",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        public PlayerHelper(ILog log, Guid serverId, IBEServer beServer, IBanHelper banHelper)
        {
            _log = log;
            _serverId = serverId;
            _beServer = beServer;
            _banHelper = banHelper;
        }

        public void RegisterPlayers(IEnumerable<Player> list)
        {
            Task.Run(() => RegisterPlayersInternal(list));
        }

        public bool RegisterPlayersInternal(IEnumerable<Player> list)
        {
            var players = list.ToList();

            if (!HaveChanges(players, x => x.Num))
            {
                return false;
            }

            using (var context = PlayerRepositoryFactory.Create())
            {
                var guids = players.Select(x => x.Guid).ToList();

                var playersInDb = context.GetPlayers(guids);
                var dbGuids = playersInDb.Select(x => x.GUID).ToList();

                var historyToAdd = new List<PlayerHistory>();
                var playerToUpdate = new List<PlayerDto>();

                foreach (var player in playersInDb)
                {
                    var p = players.FirstOrDefault(x => x.Guid == player.GUID);
                    if (p != null)
                    {
                        bool needUpdate = false;
                        if (player.Name != p.Name || player.LastIp != p.IP)
                        {
                            historyToAdd.Add(new PlayerHistory
                            {
                                IP = player.LastIp,
                                Name = player.Name,
                                PlayerId = player.Id,
                                ServerId = _serverId
                            });

                            player.Name = p.Name;
                            player.LastIp = p.IP;

                            needUpdate = true;
                        }
                        if ((DateTime.UtcNow - player.LastSeen).TotalHours > 2)
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
                {
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
                }

                context.AddOrUpdate(playerToUpdate);
                context.AddHistory(historyToAdd);
            }

            return true;
        }

        public IEnumerable<PlayerView> GetPlayerView(IEnumerable<Player> list)
        {
            using (var context = PlayerRepositoryFactory.Create())
            {
                var players = list.ToList();
                var guids = players.Select(x => x.Guid).ToList();

                var playersInDb = context.GetPlayers(guids);

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
                    }
                });

                var filterUsers = result.FirstOrDefault(x => !NameRegex.IsMatch(x.Name));
                if (filterUsers != null)
                {
#pragma warning disable 4014
                    _banHelper.Kick(_beServer, filterUsers.Num, filterUsers.Guid, "bot: Fill Nickname");
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
                        _banHelper.Kick(_beServer, bad.Num, bad.Guid, "bot: Bad Nickname");
#pragma warning restore 4014
                }


                return result;
            }
        }


    }
}