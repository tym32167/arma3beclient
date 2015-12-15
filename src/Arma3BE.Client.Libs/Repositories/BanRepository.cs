using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Arma3BEClient.Common.Core;
using Arma3BEClient.Libs.Context;
using Arma3BEClient.Libs.ModelCompact;

namespace Arma3BEClient.Libs.Repositories
{
    public class BanRepository : DisposeObject
    {
        public IEnumerable<Ban> GetActiveBans(Guid serverId, string[] playerGuids)
        {
            using (var context = new Arma3BeClientContext())
            {
                return
                    context.Bans.Where(x => x.IsActive && x.ServerId == serverId && playerGuids.Contains(x.GuidIp))
                        .ToList();
            }
        }

        public IEnumerable<Ban> GetActivePermBans()
        {
            using (var dc = new Arma3BeClientContext())
            {
                return dc.Bans
                    .Where(x => x.ServerInfo.Active && x.IsActive && x.MinutesLeft == 0)
                    .Include(x=>x.Player)
                    .ToList();
            }
        }

        public void AddOrUpdate(IEnumerable<Ban> bans)
        {
            using (var dc = new Arma3BeClientContext())
            {
                dc.Bans.AddOrUpdate(bans.ToArray());
            }
        }
    }
}