using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Arma3BEClient.Libs.Core.Model;
using Arma3BEClient.Libs.EF.Model;

namespace Arma3BEClient.Libs.Core
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
}