using Arma3BEClient.Common.Core;
using Arma3BEClient.Common.Extensions;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Libs.ModelCompact;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Arma3BEClient.Libs.Repositories.Players
{
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
}