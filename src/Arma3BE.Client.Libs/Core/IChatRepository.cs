using Arma3BE.Server.Models;
using Arma3BEClient.Libs.EF.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Arma3BEClient.Libs.Core
{
    public interface IChatRepository : IDisposable
    {
        Task AddOrUpdateAsync(ChatMessage message, Guid serverId);
        Task<IQueryable<ChatLog>> GetChatLogsAsync(string selectedServers, DateTime? startDate, DateTime? endDate, string filter);
    }
}