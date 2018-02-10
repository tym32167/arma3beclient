using Arma3BEClient.Libs.EF.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arma3BEClient.Libs.Core
{
    public interface IBanRepository : IDisposable
    {
        Task AddOrUpdateAsync(IEnumerable<Ban> bans);
        Task<IEnumerable<Ban>> GetActiveBansAsync(Guid serverId);
        Task<IEnumerable<Ban>> GetActivePermBansAsync();
    }
}