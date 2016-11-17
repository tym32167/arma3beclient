using Arma3BEClient.Libs.Context;
using Arma3BEClient.Libs.ModelCompact;
using System;
using System.Linq;

namespace Arma3BEClient.Libs.Repositories
{
    public class ReasonRepository : IDisposable
    {
        public string[] GetBanReasons()
        {
            using (var dc = new Arma3BeClientContext())
                return dc.BanReasons.Select(x => x.Text).ToArray();
        }

        public BanTime[] GetBanTimes()
        {
            using (var dc = new Arma3BeClientContext())
                return dc.BanTimes.ToArray();
        }


        public string[] GetKickReasons()
        {
            using (var dc = new Arma3BeClientContext())
                return dc.KickReasons.Select(x => x.Text).ToArray();
        }

        public void Dispose()
        {
        }

        public void UpdateBanReasons(string[] banReasons)
        {
            using (var dc = new Arma3BeClientContext())
            {
                dc.BanReasons.RemoveRange(dc.BanReasons);
                dc.BanReasons.AddRange(banReasons.Distinct().Select(x => new BanReason() { Text = x }).ToArray());
                dc.SaveChanges();
            }
        }

        public void UpdateKickReasons(string[] kickReasons)
        {
            using (var dc = new Arma3BeClientContext())
            {
                dc.KickReasons.RemoveRange(dc.KickReasons);
                dc.KickReasons.AddRange(kickReasons.Distinct().Select(x => new KickReason() { Text = x }).ToArray());
                dc.SaveChanges();
            }
        }

        public void UpdateBanTimes(BanTime[] banTimes)
        {
            using (var dc = new Arma3BeClientContext())
            {
                dc.BanTimes.RemoveRange(dc.BanTimes);
                dc.BanTimes.AddRange(banTimes.Distinct().ToArray());
                dc.SaveChanges();
            }
        }
    }
}