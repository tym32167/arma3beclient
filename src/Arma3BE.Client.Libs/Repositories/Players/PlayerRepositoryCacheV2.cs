using Arma3BEClient.Common.Core;
using Arma3BEClient.Common.Extensions;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Libs.ModelCompact;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Arma3BEClient.Libs.Repositories.Players
{
    public class PlayerRepositoryCacheV2 : DisposeObject, IPlayerRepository
    {
        private readonly IPlayerRepository _inner;
        private readonly ILog _log = LogFactory.Create(new StackTrace().GetFrame(0).GetMethod().DeclaringType);
        private readonly ICache<PlayerDto> _cache;

        public PlayerRepositoryCacheV2(IPlayerRepository inner, ICacheFactory cacheFactory)
        {
            _inner = inner;
            _cache = cacheFactory.CreateCache<PlayerDto>(TimeSpan.FromMinutes(15));
        }

        public async Task<IEnumerable<PlayerDto>> GetAllPlayersAsync()
        {
            var ret = await _inner.GetAllPlayersAsync();
            foreach (var playerDto in ret)
            {
                _cache.Add(playerDto.GUID, playerDto);
            }
            return ret;
        }

        public async Task<IEnumerable<PlayerDto>> GetPlayersAsync(Expression<Func<Player, bool>> expression)
        {
            var ret = await _inner.GetPlayersAsync(expression);
            foreach (var playerDto in ret)
            {
                _cache.Add(playerDto.GUID, playerDto);
            }
            return ret;
        }

        public async Task<IEnumerable<PlayerDto>> GetPlayersAsync(IEnumerable<string> guids)
        {
            var guidsArray = guids as string[] ?? guids.ToArray();

            var result = _cache.GetAll(guidsArray).ToList();
            var foundGuids = new HashSet<string>(result.Select(x => x.GUID));
            var notFound = guidsArray.Where(x => !foundGuids.Contains(x)).ToArray();

            if (notFound.Any())
            {
                var playedsToAdd = await _inner.GetPlayersAsync(notFound);

                foreach (var playerDto in playedsToAdd)
                {
                    _cache.Add(playerDto.GUID, playerDto);
                }

                result.AddRange(playedsToAdd);
            }

            return result;
        }

        public async Task<PlayerDto> GetPlayerAsync(string guid)
        {
            var p = _cache.Get(guid);
            if (p == null)
            {
                var res = await _inner.GetPlayerAsync(guid);
                _cache.Add(res.GUID, res);
            }
            return p;
        }

        public async Task<Player> GetPlayerInfoAsync(string guid)
        {
            var p = await _inner.GetPlayerInfoAsync(guid);
            _cache.Add(p.GUID, p);
            return p;
        }

        public async Task UpdatePlayerCommentAsync(string guid, string comment)
        {
            await _inner.UpdatePlayerCommentAsync(guid, comment);
            var p = _cache.Get(guid);
            if (p != null) p.Comment = comment;
        }

        public async Task UpdateCommentAsync(Dictionary<Guid, string> playersToUpdateComments)
        {
            var dto = playersToUpdateComments ?? new Dictionary<Guid, string>();
            if (dto.Count == 0) return;

            await _inner.UpdateCommentAsync(dto);

            foreach (var player in _cache.GetAll())
            {
                if (dto.ContainsKey(player.Id))
                {
                    player.Comment = dto[player.Id];
                }
            }
        }

        public async Task AddOrUpdateAsync(IEnumerable<PlayerDto> players)
        {
            var playerDtos = players?.ToArray() ?? new PlayerDto[0];
            if (!playerDtos.Any()) return;

            await _inner.AddOrUpdateAsync(playerDtos);

            foreach (var player in playerDtos)
            {
                _cache.Remove(player.GUID);
            }
        }

        public Task AddHistoryAsync(List<PlayerHistory> histories)
        {
            return _inner.AddHistoryAsync(histories);
        }

        public Task AddNotesAsync(Guid id, string s)
        {
            return _inner.AddNotesAsync(id, s);
        }

        public Task<IEnumerable<PlayerDto>> GetAllPlayersWithoutSteamAsync()
        {
            return _inner.GetAllPlayersWithoutSteamAsync();
        }

        public async Task SaveSteamIdAsync(Dictionary<Guid, string> found)
        {
            var cachedItems = _cache.GetAll().ToDictionary(x => x.Id);

            foreach (var paged in found.Paged(2000))
            {
                await _inner.SaveSteamIdAsync(paged.ToDictionary(x => x.Key, x => x.Value));

                foreach (var player in paged)
                {
                    if (cachedItems.ContainsKey(player.Key))
                        _cache.Remove(cachedItems[player.Key].GUID);
                }
            }

            _log.Info("SaveSteamIdAsync resetting cache");
        }

        public async Task ImportPlayersAsync(List<PlayerDto> toAdd, List<PlayerDto> toUpdate)
        {
            if (toAdd.Any() || toUpdate.Any())
            {
                await _inner.ImportPlayersAsync(toAdd, toUpdate);


                foreach (var playerDto in toAdd)
                {
                    _cache.Remove(playerDto.GUID);
                }

                foreach (var playerDto in toUpdate)
                {
                    _cache.Remove(playerDto.GUID);
                }

                _log.Info("ImportPlayersAsync resetting cache");
            }
        }

        protected override void DisposeManagedResources()
        {
            _inner?.Dispose();
            base.DisposeManagedResources();
        }
    }
}