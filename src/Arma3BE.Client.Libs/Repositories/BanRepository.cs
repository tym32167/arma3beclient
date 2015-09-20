using System;
using System.Collections.Generic;
using System.Linq;
using Arma3BEClient.Libs.Context;
using Arma3BEClient.Libs.ModelCompact;

namespace Arma3BEClient.Libs.Repositories
{
    public class BanRepository : IDisposable
    {
        public void Dispose()
        {
        }

        public IEnumerable<Ban> GetActivePermBans()
        {
            using (var dc = new Arma3BeClientContext())
            {
                return dc.Bans.Where(x => x.ServerInfo.Active && x.IsActive && x.MinutesLeft == 0).ToList();
            }
        }
    }
}