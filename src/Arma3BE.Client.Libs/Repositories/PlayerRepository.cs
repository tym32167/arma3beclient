using Arma3BEClient.Common.Core;
using Arma3BEClient.Libs.Context;
using Arma3BEClient.Libs.ModelCompact;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Arma3BEClient.Libs.Repositories
{
    public class PlayerRepositoryFactory : IFactory<PlayerRepositoryFactory.IPlayerRepository>
    {
        public static IPlayerRepository Create()
        {
            return new PlayerRepositoryCache(new PlayerRepository());
        }

        public interface IPlayerRepository : IDisposable
        {
            IEnumerable<PlayerDto> GetAllPlayers();
            IEnumerable<PlayerDto> GetPlayers(Expression<Func<Player, bool>> expression);
            IEnumerable<PlayerDto> GetPlayers(IEnumerable<string> guids);
            PlayerDto GetPlayer(string guid);
            Player GetPlayerInfo(string guid);

            void AddPlayers(IEnumerable<PlayerDto> players);
            void UpdatePlayerComment(string guid, string comment);
            void UpdateCommant(Dictionary<Guid, string> playersToUpdateComments);
            void AddOrUpdate(IEnumerable<PlayerDto> players);
            void AddHistory(List<PlayerHistory> histories);
            void AddNotes(Guid id, string s);
        }

        private class PlayerRepositoryCache : DisposeObject, IPlayerRepository
        {
            private static readonly object Lock = new object();
            private readonly IPlayerRepository _playerRepository;
            private static volatile bool _validCache;
            private static volatile ConcurrentDictionary<string, PlayerDto> _playersByGuidsCache;

            private void ResetCache()
            {
                if (_validCache) return;
                lock (Lock)
                {
                    if (_validCache) return;
                    var playersByGuidsCache = new ConcurrentDictionary<string, PlayerDto>();
                    var players = _playerRepository.GetAllPlayers().GroupBy(x => x.GUID).Select(x => x.First());
                    foreach (var playerDto in players)
                    {
                        playersByGuidsCache.AddOrUpdate(playerDto.GUID, playerDto, (key, value) => value);
                    }

                    _playersByGuidsCache = playersByGuidsCache;
                    _validCache = true;
                }
            }

            public PlayerRepositoryCache(IPlayerRepository playerRepository)
            {
                _playerRepository = playerRepository;
                Task.Run(() => ResetCache());
            }

            public IEnumerable<PlayerDto> GetAllPlayers()
            {
                if (!_validCache) ResetCache();
                return _playersByGuidsCache.Values.ToList();
            }

            public IEnumerable<PlayerDto> GetPlayers(Expression<Func<Player, bool>> expression)
            {
                return _playerRepository.GetPlayers(expression);
            }

            public IEnumerable<PlayerDto> GetPlayers(IEnumerable<string> guids)
            {
                if (!_validCache) ResetCache();

                var result = new List<PlayerDto>();
                PlayerDto element;

                foreach (var guid in guids.Distinct().ToArray())
                {
                    if (_playersByGuidsCache.TryGetValue(guid, out element))
                        result.Add(element);
                }

                return result;
            }

            public PlayerDto GetPlayer(string guid)
            {
                PlayerDto element;
                if (_playersByGuidsCache.TryGetValue(guid, out element))
                    return element;
                return null;
            }

            public Player GetPlayerInfo(string guid)
            {
                return _playerRepository.GetPlayerInfo(guid);
            }

            public void AddPlayers(IEnumerable<PlayerDto> players)
            {
                var playerDtos = players?.ToArray() ?? new PlayerDto[0];
                if (!playerDtos.Any()) return;

                _playerRepository.AddPlayers(playerDtos);
                _validCache = false;
            }

            public void UpdatePlayerComment(string guid, string comment)
            {
                _playerRepository.UpdatePlayerComment(guid, comment);

                PlayerDto element;
                if (_playersByGuidsCache.TryGetValue(guid, out element))
                {
                    element.Comment = comment;
                }
            }

            public void UpdateCommant(Dictionary<Guid, string> playersToUpdateComments)
            {
                var dto = playersToUpdateComments ?? new Dictionary<Guid, string>();
                if (dto.Count == 0) return;

                _playerRepository.UpdateCommant(dto);

                PlayerDto element;
                var local = _playersByGuidsCache.Values.ToDictionary(x => x.Id);

                foreach (var playersToUpdateComment in dto)
                {
                    if (local.TryGetValue(playersToUpdateComment.Key, out element))
                    {
                        element.Comment = playersToUpdateComment.Value;
                    }
                }
            }

            public void AddOrUpdate(IEnumerable<PlayerDto> players)
            {
                var playerDtos = players?.ToArray() ?? new PlayerDto[0];
                if (!playerDtos.Any()) return;

                _playerRepository.AddOrUpdate(playerDtos);
                _validCache = false;
            }

            public void AddHistory(List<PlayerHistory> histories)
            {
                _playerRepository.AddHistory(histories);
            }

            public void AddNotes(Guid id, string s)
            {
                _playerRepository.AddNotes(id, s);
            }
        }


        private class PlayerRepository : DisposeObject, IPlayerRepository
        {
            public IEnumerable<PlayerDto> GetAllPlayers()
            {
                using (var dc = new Arma3BeClientContext())
                {
                    return dc.Player.ToList();
                }
            }

            public void AddPlayers(IEnumerable<PlayerDto> players)
            {
                var playerList = players.Select(Map).ToArray();

                using (var dc = new Arma3BeClientContext())
                {
                    dc.Player.AddRange(playerList);
                    dc.SaveChanges();
                }
            }

            public IEnumerable<PlayerDto> GetPlayers(Expression<Func<Player, bool>> expression)
            {
                using (var dc = new Arma3BeClientContext())
                {
                    return dc.Player.Where(expression).ToList();
                }
            }

            public IEnumerable<PlayerDto> GetPlayers(IEnumerable<string> guids)
            {
                using (var dc = new Arma3BeClientContext())
                {
                    return dc.Player.Where(x => guids.Contains(x.GUID)).ToList();
                }
            }

            public PlayerDto GetPlayer(string guid)
            {
                using (var dc = new Arma3BeClientContext())
                {
                    return dc.Player.FirstOrDefault(x => x.GUID == guid);
                }
            }

            public Player GetPlayerInfo(string guid)
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
            }

            public void UpdatePlayerComment(string guid, string comment)
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
            }

            public void UpdateCommant(Dictionary<Guid, string> playersToUpdateComments)
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
            }

            public void AddOrUpdate(IEnumerable<PlayerDto> players)
            {
                var playerList = players.Select(Map).ToArray();

                using (var dc = new Arma3BeClientContext())
                {
                    dc.Player.AddOrUpdate(playerList);
                    dc.SaveChanges();
                }
            }

            public void AddHistory(List<PlayerHistory> histories)
            {
                using (var dc = new Arma3BeClientContext())
                {
                    dc.PlayerHistory.AddOrUpdate(histories.ToArray());
                    dc.SaveChanges();
                }
            }

            public void AddNotes(Guid id, string s)
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
                    Name = source.Name
                };
            }
        }

        IPlayerRepository IFactory<IPlayerRepository>.Create()
        {
            return Create();
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