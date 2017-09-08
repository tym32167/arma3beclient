using Arma3BEClient.Common.Core;
using Arma3BEClient.Common.Extensions;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Libs.Context;
using Arma3BEClient.Libs.ModelCompact;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Arma3BEClient.Libs.Repositories
{
    public interface IPlayerRepository : IDisposable
    {
        Task<IEnumerable<PlayerDto>> GetAllPlayersAsync();
        Task<IEnumerable<PlayerDto>> GetPlayersAsync(Expression<Func<Player, bool>> expression);
        Task<IEnumerable<PlayerDto>> GetPlayersAsync(IEnumerable<string> guids);
        Task<PlayerDto> GetPlayerAsync(string guid);
        Task<Player> GetPlayerInfoAsync(string guid);

        Task UpdatePlayerCommentAsync(string guid, string comment);
        Task UpdateCommentAsync(Dictionary<Guid, string> playersToUpdateComments);
        Task AddOrUpdateAsync(IEnumerable<PlayerDto> players);
        Task AddHistoryAsync(List<PlayerHistory> histories);
        Task AddNotesAsync(Guid id, string s);
        Task<IEnumerable<PlayerDto>> GetAllPlayersWithoutSteamAsync();
        Task SaveSteamIdAsync(Dictionary<Guid, string> found);

        Task ImportPlayersAsync(List<PlayerDto> toAdd, List<PlayerDto> toUpdate);
    }

    public class PlayerRepositoryCache : DisposeObject, IPlayerRepository
    {
        private static readonly object Lock = new object();
        private readonly IPlayerRepository _playerRepository;
        private readonly ILog _log = LogFactory.Create(new StackTrace().GetFrame(0).GetMethod().DeclaringType);
        private static volatile bool _validCache;
        private static volatile ConcurrentDictionary<string, PlayerDto> _playersByGuidsCache;


        async Task<ConcurrentDictionary<string, PlayerDto>> GetCacheAsync()
        {
            if (!_validCache) await ResetCacheAsync();
            return _playersByGuidsCache;
        }


        private void ResetCache()
        {
            if (_validCache) return;
            lock (Lock)
            {

                if (_validCache) return;
                _log.Info($"Refresh Players cache from {new StackTrace()}.");
                var playersByGuidsCache = new ConcurrentDictionary<string, PlayerDto>();
                var players = _playerRepository.GetAllPlayersAsync().Result.GroupBy(x => x.GUID).Select(x => x.First());
                foreach (var playerDto in players)
                {
                    playersByGuidsCache.AddOrUpdate(playerDto.GUID, playerDto, (key, value) => value);
                }

                _playersByGuidsCache = playersByGuidsCache;
                _validCache = true;
            }
        }

        private Task ResetCacheAsync()
        {
            return Task.Run(() => ResetCache());
        }

        public PlayerRepositoryCache(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public async Task<IEnumerable<PlayerDto>> GetAllPlayersAsync()
        {
            return (await GetCacheAsync()).Values.ToArray();
        }

        public Task<IEnumerable<PlayerDto>> GetPlayersAsync(Expression<Func<Player, bool>> expression)
        {
            return _playerRepository.GetPlayersAsync(expression);
        }

        public async Task<IEnumerable<PlayerDto>> GetPlayersAsync(IEnumerable<string> guids)
        {
            var result = new List<PlayerDto>();

            foreach (var guid in guids.Distinct().ToArray())
            {
                PlayerDto element;
                if ((await GetCacheAsync()).TryGetValue(guid, out element))
                    result.Add(element);
            }

            return result;
        }

        public async Task<PlayerDto> GetPlayerAsync(string guid)
        {
            PlayerDto element;
            if ((await GetCacheAsync()).TryGetValue(guid, out element))
                return element;
            return null;
        }

        public Task<Player> GetPlayerInfoAsync(string guid)
        {
            return _playerRepository.GetPlayerInfoAsync(guid);
        }

        public async Task UpdatePlayerCommentAsync(string guid, string comment)
        {
            await _playerRepository.UpdatePlayerCommentAsync(guid, comment);

            PlayerDto element;
            if ((await GetCacheAsync()).TryGetValue(guid, out element))
            {
                element.Comment = comment;
            }
        }

        public async Task UpdateCommentAsync(Dictionary<Guid, string> playersToUpdateComments)
        {
            var dto = playersToUpdateComments ?? new Dictionary<Guid, string>();
            if (dto.Count == 0) return;

            await _playerRepository.UpdateCommentAsync(dto);

            var local = (await GetCacheAsync()).Values.ToDictionary(x => x.Id);

            foreach (var playersToUpdateComment in dto)
            {
                PlayerDto element;
                if (local.TryGetValue(playersToUpdateComment.Key, out element))
                {
                    element.Comment = playersToUpdateComment.Value;
                }
            }
        }

        public async Task AddOrUpdateAsync(IEnumerable<PlayerDto> players)
        {
            var playerDtos = players?.ToArray() ?? new PlayerDto[0];
            if (!playerDtos.Any()) return;

            await _playerRepository.AddOrUpdateAsync(playerDtos);

            foreach (var player in playerDtos)
            {
                if ((await GetCacheAsync()).ContainsKey(player.GUID) == false)
                {
                    _log.Info("AddOrUpdateAsync - found new Player, resetting cache");
                    _validCache = false;
                    break;
                }

                (await GetCacheAsync()).AddOrUpdate(player.GUID, player, (k, v) => player);
            }
        }

        public Task AddHistoryAsync(List<PlayerHistory> histories)
        {
            return _playerRepository.AddHistoryAsync(histories);
        }

        public Task AddNotesAsync(Guid id, string s)
        {
            return _playerRepository.AddNotesAsync(id, s);
        }

        public Task<IEnumerable<PlayerDto>> GetAllPlayersWithoutSteamAsync()
        {
            if (_validCache)
            {
                return Task.FromResult(_playersByGuidsCache.Values
                    .Where(x => string.IsNullOrEmpty(x.SteamId)));
            }
            else
            {
                return _playerRepository.GetAllPlayersWithoutSteamAsync();
            }
        }

        public async Task SaveSteamIdAsync(Dictionary<Guid, string> found)
        {
            foreach (var paged in found.Paged(2000))
            {
                await _playerRepository.SaveSteamIdAsync(paged.ToDictionary(x => x.Key, x => x.Value));
            }

            _log.Info("SaveSteamIdAsync resetting cache");
            _validCache = false;
        }

        public async Task ImportPlayersAsync(List<PlayerDto> toAdd, List<PlayerDto> toUpdate)
        {
            if (toAdd.Any() || toUpdate.Any())
            {
                await _playerRepository.ImportPlayersAsync(toAdd, toUpdate);

                _log.Info("ImportPlayersAsync resetting cache");
                _validCache = false;
            }
        }
    }

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

    public class PlayerDto
    {
        public PlayerDto()
        {
            LastSeen = DateTime.UtcNow;
        }

        [Key]
        public Guid Id { get; set; }

        public string GUID { get; set; }

        public string SteamId { get; set; }

        public string Name { get; set; }
        public string Comment { get; set; }

        public string LastIp { get; set; }
        public DateTime LastSeen { get; set; }
    }
}