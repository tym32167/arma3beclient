using Arma3BEClient.Libs.Core;
using Arma3BEClient.Libs.Core.Model;
using Arma3BEClient.Libs.RavenDB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arma3BEClient.Libs.RavenDB.Repositories
{
    public class ServerInfoRepository : RepositoryBase, IServerInfoRepository
    {
        public Task AddOrUpdateAsync(ServerInfoDto serverInfo)
        {
            using (var session = CreateStore().OpenSession())
            {
                session.Store(FromDto(serverInfo));
                session.SaveChanges();
                return Task.CompletedTask;
            }
        }

        public Task<IEnumerable<ServerInfoDto>> GetActiveServerInfoAsync()
        {
            using (var session = CreateStore().OpenSession())
            {
                var servers = session.Query<ServerInfo>().Where(s => s.Active).Select(FromModel).ToArray();
                return Task.FromResult<IEnumerable<ServerInfoDto>>(servers);
            }
        }

        public Task<IEnumerable<ServerInfoDto>> GetNotActiveServerInfoAsync()
        {
            using (var session = CreateStore().OpenSession())
            {
                var servers = session.Query<ServerInfo>().Where(s => s.Active == false).Select(FromModel).ToArray();
                return Task.FromResult<IEnumerable<ServerInfoDto>>(servers);
            }
        }

        public Task<IEnumerable<ServerInfoDto>> GetServerInfoAsync()
        {
            using (var session = CreateStore().OpenSession())
            {
                var servers = session.Query<ServerInfo>().Select(FromModel).ToArray();
                return Task.FromResult<IEnumerable<ServerInfoDto>>(servers);
            }
        }

        public Task RemoveAsync(Guid serverInfoId)
        {
            using (var session = CreateStore().OpenSession())
            {
                session.Delete(serverInfoId.ToString());
                session.SaveChanges();
                return Task.CompletedTask;
            }
        }

        public Task SetServerInfoActiveAsync(Guid serverInfoId, bool active)
        {
            using (var session = CreateStore().OpenSession())
            {
                var si = session.Load<ServerInfo>(serverInfoId.ToString());
                si.Active = active;
                session.Store(si);
                session.SaveChanges();
                return Task.CompletedTask;
            }
        }

        private ServerInfo FromDto(ServerInfoDto dto)
        {
            return new ServerInfo()
            {
                Id = dto.Id.ToString(),
                Active = dto.Active,
                Host = dto.Host,
                Name = dto.Name,
                Password = dto.Password,
                Port = dto.Port,
                SteamPort = dto.SteamPort
            };
        }

        private ServerInfoDto FromModel(ServerInfo dto)
        {
            return new ServerInfoDto()
            {
                Id = Guid.Parse(dto.Id),
                Active = dto.Active,
                Host = dto.Host,
                Name = dto.Name,
                Password = dto.Password,
                Port = dto.Port,
                SteamPort = dto.SteamPort
            };
        }
    }
}