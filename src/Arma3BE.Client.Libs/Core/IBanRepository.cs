using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Arma3BEClient.Libs.EF.Model;

namespace Arma3BEClient.Libs.Core
{
    public interface IBanRepository
    {
        Task AddOrUpdateAsync(IEnumerable<Ban> bans);
        Task<IEnumerable<Ban>> GetActiveBansAsync(Guid serverId);
        Task<IEnumerable<Ban>> GetActivePermBansAsync();
    }
}