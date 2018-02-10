using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Arma3BEClient.Common.Core;
using Arma3BEClient.Common.Extensions;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Libs.Core;
using Arma3BEClient.Libs.EF.Context;
using Arma3BEClient.Libs.EF.Model;

namespace Arma3BEClient.Libs.EF.Repositories.Players
{
    public class PlayerRepository : DisposeObject, IPlayerRepository
    {
        private readonly ILog _log = new Log();

        public async Task<IEnumerable<PlayerDto>> GetAllPlayersAsync()
        {
            using (var dc = new Arma3BeClientContext())
            {
                return await dc.Player.ToListAsync();
            }
        }

        public Task ImportPlayersAsync(List<PlayerDto> toAdd, List<PlayerDto> toUpdate)
        {
            return Task.Run(() =>
            {
                _log.Info("ImportPlayersAsync - Import started");
                _log.Info($"ImportPlayersAsync - ToAdd {toAdd.Count}, ToUpdate {toUpdate.Count}");
                using (var dc = new Arma3BeClientContext())
                {
                    foreach (var _add in toAdd.Paged(2000))
                    {
                        var toAddDto = _add.Select(Map).ToArray();
                        dc.Player.AddRange(toAddDto);
                        dc.SaveChanges();
                    }
                }

                _log.Info($"Inserted {toAdd.Count}");

                _log.Info("ImportPlayersAsync - Update started");
                using (var dc = new Arma3BeClientContext())
                {
                    var allPlayers = dc.Player.ToArray().ToDictionary(x => x.Id);

                    _log.Info($"Players loaded");

                    foreach (var player in toUpdate)
                    {
                        if (allPlayers.ContainsKey(player.Id))
                        {
                            var dto = allPlayers[player.Id];

                            if (string.IsNullOrEmpty(dto.SteamId))
                                dto.SteamId = player.SteamId;

                            if (string.IsNullOrEmpty(dto.Comment))
                                dto.Comment = player.Comment;
                        }
                    }

                    dc.SaveChanges();
                }

                _log.Info("ImportPlayersAsync - Update finished");
            });
        }

        public async Task<IEnumerable<PlayerDto>> GetPlayersAsync(Expression<Func<Player, bool>> expression)
        {
            return await Task.Run(() =>
            {
                using (var dc = new Arma3BeClientContext())
                {
                    return dc.Player.Where(expression).ToList();
                }
            });
        }

        public async Task<IEnumerable<PlayerDto>> GetPlayersAsync(IEnumerable<string> guids)
        {
            return await Task.Run(() =>
            {
                using (var dc = new Arma3BeClientContext())
                {
                    return dc.Player.Where(x => guids.Contains(x.GUID)).ToList();
                }
            });
        }

        public async Task<PlayerDto> GetPlayerAsync(string guid)
        {
            return await Task.Run(() =>
            {
                using (var dc = new Arma3BeClientContext())
                {
                    return dc.Player.FirstOrDefault(x => x.GUID == guid);
                }
            });
        }

        public Task<Player> GetPlayerInfoAsync(string guid)
        {
            return Task.Run(() =>
            {
                using (var dc = new Arma3BeClientContext())
                {
                    return
                        dc.Player.Where(x => x.GUID == guid)
                            .Include(x => x.Bans)
                            .Include(x => x.Bans.Select(b => b.ServerInfo))
                            .Include(x => x.Notes)
                            .Include(x => x.PlayerHistory)
                            .FirstOrDefault();
                }
            });
        }

        public Task UpdatePlayerCommentAsync(string guid, string comment)
        {
            return Task.Run(() =>
            {
                using (var dc = new Arma3BeClientContext())
                {
                    var dbp = dc.Player.FirstOrDefault(x => x.GUID == guid);
                    if (dbp != null)
                    {
                        dbp.Comment = comment;
                        dc.SaveChanges();
                    }
                }
            });
        }

        public Task UpdateCommentAsync(Dictionary<Guid, string> playersToUpdateComments)
        {
            return Task.Run(() =>
            {
                var ids = playersToUpdateComments.Keys.ToArray();

                using (var dc = new Arma3BeClientContext())
                {
                    var players = dc.Player.Where(x => ids.Contains(x.Id));
                    foreach (var player in players)
                    {
                        player.Comment = playersToUpdateComments[player.Id];
                    }
                    dc.SaveChanges();
                }
            });
        }

        public Task AddOrUpdateAsync(IEnumerable<PlayerDto> players)
        {
            return Task.Run(() =>
            {
                var playerList = players.Select(Map).ToArray();

                using (var dc = new Arma3BeClientContext())
                {
                    dc.Player.AddOrUpdate(playerList);
                    dc.SaveChanges();
                }
            });
        }

        public Task AddHistoryAsync(List<PlayerHistory> histories)
        {
            return Task.Run(() =>
            {
                using (var dc = new Arma3BeClientContext())
                {
                    dc.PlayerHistory.AddOrUpdate(histories.ToArray());
                    dc.SaveChanges();
                }
            });
        }

        public Task AddNotesAsync(Guid id, string s)
        {
            return Task.Run(() =>
            {
                using (var dc = new Arma3BeClientContext())
                {
                    dc.Comments.Add(new Note()
                    {
                        PlayerId = id,
                        Text = s,
                    });
                    dc.SaveChanges();
                }
            });
        }

        public async Task<IEnumerable<PlayerDto>> GetAllPlayersWithoutSteamAsync()
        {
            return await Task.Run(() =>
            {
                using (var dc = new Arma3BeClientContext())
                {
                    return
                        dc.Player.Where(x => string.IsNullOrEmpty(x.SteamId)).ToArray();
                }
            });
        }

        public Task SaveSteamIdAsync(Dictionary<Guid, string> found)
        {
            return Task.Run(() =>
            {
                var guids = found.Keys.ToArray();
                using (var dc = new Arma3BeClientContext())
                {
                    var players = dc.Player
                        .Where(x => string.IsNullOrEmpty(x.GUID) == false && string.IsNullOrEmpty(x.SteamId))
                        .Where(x => guids.Contains(x.Id)).ToArray();

                    foreach (var player in players)
                    {
                        if (found.ContainsKey(player.Id))
                        {
                            player.SteamId = found[player.Id];
                        }
                    }

                    dc.SaveChanges();
                }
            });
        }


        private Player Map(PlayerDto source)
        {
            return new Player()
            {
                Id = source.Id,
                Comment = source.Comment,
                GUID = source.GUID,
                LastIp = source.LastIp,
                LastSeen = source.LastSeen,
                Name = source.Name,
                SteamId = source.SteamId
            };
        }
    }
}