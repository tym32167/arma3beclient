using Arma3BE.Server.Models;
using Arma3BEClient.Common.Core;
using Arma3BEClient.Libs.Context;
using Arma3BEClient.Libs.ModelCompact;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Arma3BEClient.Libs.Repositories
{
    public class ChatRepository : DisposeObject
    {
        public Task AddOrUpdateAsync(ChatMessage message, Guid serverId)
        {
            return Task.Run(() =>
            {

                using (var context = new Arma3BeClientContext())
                {
                    context.ChatLog.Add(new ChatLog
                    {
                        Date = message.Date,
                        ServerId = serverId,
                        Text = message.Message
                    });

                    context.SaveChangesAsync();
                }
            });
        }


        public Task<IQueryable<ChatLog>> GetChatLogsAsync(string selectedServers, DateTime? startDate, DateTime? endDate,
            string filter)
        {
            return Task.Run(() =>
            {
                using (var dc = new Arma3BeClientContext())
                {
                    var log = dc.ChatLog.AsQueryable();

                    if (!string.IsNullOrEmpty(selectedServers))
                    {
                        var guids = selectedServers.Split(',').Select(Guid.Parse).ToArray();
                        if (guids.Length > 0)
                        {
                            log = log.Where(x => guids.Contains(x.ServerId));
                        }
                    }

                    if (startDate.HasValue)
                    {
                        var date = startDate.Value;
                        log = log.Where(x => x.Date >= date);
                    }

                    if (endDate.HasValue)
                    {
                        var date = endDate.Value;
                        log = log.Where(x => x.Date <= date);
                    }

                    if (!string.IsNullOrEmpty(filter))
                    {
                        log = log.Where(x => x.Text.Contains(filter));
                    }
                    return log.Include(x => x.ServerInfo).ToArray().AsQueryable();
                }
            });
        }
    }
}