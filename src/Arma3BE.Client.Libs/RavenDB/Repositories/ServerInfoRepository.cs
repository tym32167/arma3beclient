using Arma3BEClient.Libs.Core;
using Arma3BEClient.Libs.Core.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arma3BEClient.Libs.RavenDB.Repositories
{
    public class ServerInfoRepository : RepositoryBase, IServerInfoRepository
    {
        public Task AddOrUpdateAsync(ServerInfoDto serverInfo)
        {
            using (var store = CreateStore())
            {
                throw new NotImplementedException();
            }
        }

        public Task<IEnumerable<ServerInfoDto>> GetActiveServerInfoAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ServerInfoDto>> GetNotActiveServerInfoAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ServerInfoDto>> GetServerInfoAsync()
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(Guid serverInfoId)
        {
            throw new NotImplementedException();
        }

        public Task SetServerInfoActiveAsync(Guid serverInfoId, bool active)
        {
            throw new NotImplementedException();
        }
    }
}