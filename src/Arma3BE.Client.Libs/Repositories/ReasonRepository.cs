using System;
using System.Linq;
using Arma3BEClient.Libs.Context;
using Arma3BEClient.Libs.ModelCompact;

namespace Arma3BEClient.Libs.Repositories
{
    public class ReasonRepository : IDisposable
    {
        public string[] GetBanReasons()
        {
            using (var dc = new Arma3BeClientContext())
                return dc.BanReasons.Select(x=>x.Text).ToArray();
        }

        public BanTime[] GetBanTimes()
        {
            using (var dc = new Arma3BeClientContext())
                return dc.BanTimes.ToArray();
        }


        public string[] GetKickReasons()
        {
            using (var dc = new Arma3BeClientContext())
                return dc.KickReasons.Select(x=>x.Text).ToArray();
        }

        public void Dispose()
        {
        }
    }
}