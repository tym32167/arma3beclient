using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Arma3BEClient.Libs.Core.Model;
using Arma3BEClient.Libs.EF.Repositories;

namespace Arma3BEClient.Libs.Core
{
    public interface IServerInfoRepository
    {
        Task AddOrUpdateAsync(ServerInfoDto serverInfo);
        Task<IEnumerable<ServerInfoDto>> GetActiveServerInfoAsync();
        Task<IEnumerable<ServerInfoDto>> GetNotActiveServerInfoAsync();
        Task<IEnumerable<ServerInfoDto>> GetServerInfoAsync();
        Task RemoveAsync(Guid serverInfoId);
        Task SetServerInfoActiveAsync(Guid serverInfoId, bool active);
    }
}