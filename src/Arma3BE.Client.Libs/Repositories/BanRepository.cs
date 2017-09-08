using Arma3BEClient.Common.Core;
using Arma3BEClient.Libs.Context;
using Arma3BEClient.Libs.ModelCompact;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;

namespace Arma3BEClient.Libs.Repositories
{
    public class BanRepository : DisposeObject
    {
        public async Task<IEnumerable<Ban>> GetActiveBansAsync(Guid serverId)
        {
            return await Task.Run(() =>
            {
                using (var context = new Arma3BeClientContext())
                {
                    return
                        context.Bans.Where(x => x.IsActive && x.ServerId == serverId)
                            .ToList();
                }
            });
        }

        public async Task<IEnumerable<Ban>> GetActivePermBansAsync()
        {
            return await Task.Run(() =>
            {
                using (var dc = new Arma3BeClientContext())
                {
                    return dc.Bans
                        .Where(x => x.ServerInfo.Active && x.IsActive && x.MinutesLeft == 0)
                        .Include(x => x.Player)
                        .ToList();
                }
            });
        }

        public Task AddOrUpdateAsync(IEnumerable<Ban> bans)
        {
            return Task.Run(() =>
            {
                using (var dc = new Arma3BeClientContext())
                {
                    dc.Bans.AddOrUpdate(bans.ToArray());
                    dc.SaveChanges();
                }
            });
        }
    }
}