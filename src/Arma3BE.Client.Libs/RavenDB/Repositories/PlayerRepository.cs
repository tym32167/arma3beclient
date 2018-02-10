using Arma3BEClient.Libs.Core;
using Arma3BEClient.Libs.Core.Model;
using Arma3BEClient.Libs.EF.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Arma3BEClient.Libs.RavenDB.Repositories
{
    public class PlayerRepository : RepositoryBase, IPlayerRepository
    {
        public void Dispose()
        {

        }

        public Task<IEnumerable<PlayerDto>> GetAllPlayersAsync()
        {
            using (var session = CreateStore().OpenSession())
            {
                var res = session.Query<Model.Player>().Select(FromModel).ToArray();
                return Task.FromResult<IEnumerable<PlayerDto>>(res);
            }
        }

        public Task<IEnumerable<PlayerDto>> GetPlayersAsync(Expression<Func<Player, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PlayerDto>> GetPlayersAsync(IEnumerable<string> guids)
        {
            using (var session = CreateStore().OpenSession())
            {
                var res = session.Query<Model.Player>()
                    .Where(x => guids.Contains(x.GUID))
                    .Select(FromModel)
                    .ToArray();
                return Task.FromResult<IEnumerable<PlayerDto>>(res);
            }
        }

        public Task<PlayerDto> GetPlayerAsync(string guid)
        {
            using (var session = CreateStore().OpenSession())
            {
                var res = session.Query<Model.Player>()
                    .Where(x => x.GUID == guid)
                    .Select(FromModel)
                    .FirstOrDefault();
                return Task.FromResult(res);
            }
        }

        public Task<Player> GetPlayerInfoAsync(string guid)
        {
            throw new NotImplementedException();
        }

        public Task UpdatePlayerCommentAsync(string guid, string comment)
        {
            using (var session = CreateStore().OpenSession())
            {
                var res = session.Query<Model.Player>()
                    .Where(x => x.GUID == guid)
                    .Select(FromModel)
                    .FirstOrDefault();
                if (res != null)
                {
                    res.Comment = comment;
                    session.Store(res);
                    session.SaveChanges();
                }

                return Task.CompletedTask;
            }
        }

        public Task UpdateCommentAsync(Dictionary<Guid, string> playersToUpdateComments)
        {
            throw new NotImplementedException();
        }

        public Task AddOrUpdateAsync(IEnumerable<PlayerDto> players)
        {
            using (var session = CreateStore().OpenSession())
            {
                foreach (var player in players.Select(FromDto))
                {
                    session.Store(player);
                }

                return Task.CompletedTask;
            }
        }

        public Task AddHistoryAsync(List<PlayerHistory> histories)
        {
            return Task.CompletedTask;
        }

        public Task AddNotesAsync(Guid id, string s)
        {
            return Task.CompletedTask;
        }

        public Task<IEnumerable<PlayerDto>> GetAllPlayersWithoutSteamAsync()
        {
            throw new NotImplementedException();
        }

        public Task SaveSteamIdAsync(Dictionary<Guid, string> found)
        {
            throw new NotImplementedException();
        }

        public Task ImportPlayersAsync(List<PlayerDto> toAdd, List<PlayerDto> toUpdate)
        {
            throw new NotImplementedException();
        }


        private static Model.Player FromDto(PlayerDto x)
        {
            return new Model.Player()
            {
                Id = x.Id.ToString(),
                GUID = x.GUID,
                IP = x.LastIp,
                LastSeen = x.LastSeen,
                Name = x.Name,
                SteamId = x.SteamId,
                Comment = x.Comment
            };
        }

        private static PlayerDto FromModel(Model.Player x)
        {
            return new PlayerDto()
            {
                Id = Guid.Parse(x.Id),
                GUID = x.GUID,
                LastIp = x.IP,
                LastSeen = x.LastSeen,
                Name = x.Name,
                SteamId = x.SteamId,
                Comment = x.Comment
            };
        }
    }
}