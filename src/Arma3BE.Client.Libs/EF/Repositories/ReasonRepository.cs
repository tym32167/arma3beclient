using System;
using System.Linq;
using System.Threading.Tasks;
using Arma3BEClient.Libs.Core;
using Arma3BEClient.Libs.EF.Context;
using Arma3BEClient.Libs.EF.Model;

namespace Arma3BEClient.Libs.EF.Repositories
{
    public class ReasonRepository : IDisposable, IReasonRepository
    {
        public Task<string[]> GetBanReasonsAsync()
        {
            return Task.Run(() =>
            {
                using (var dc = new Arma3BeClientContext())
                    return dc.BanReasons.Select(x => x.Text).ToArray();
            });
        }

        public Task<BanTime[]> GetBanTimesAsync()
        {
            return Task.Run(() =>
            {
                using (var dc = new Arma3BeClientContext())
                    return dc.BanTimes.ToArray();
            });
        }


        public Task<string[]> GetKickReasonsAsync()
        {
            return Task.Run(() =>
            {
                using (var dc = new Arma3BeClientContext())
                    return dc.KickReasons.Select(x => x.Text).ToArray();
            });
        }

        public Task<string[]> GetBadNicknamesAsync()
        {
            return Task.Run(() =>
            {
                using (var dc = new Arma3BeClientContext())
                    return dc.BadNicknames.Select(x => x.Text).ToArray();
            });
        }

        public Task<string[]> GetImportantWordsAsync()
        {
            return Task.Run(() =>
            {
                using (var dc = new Arma3BeClientContext())
                    return dc.ImportantWords.Select(x => x.Text).ToArray();
            });
        }

        public void Dispose()
        {
        }

        public Task UpdateBadNicknames(string[] badNicknames)
        {
            return Task.Run(() =>
            {
                using (var dc = new Arma3BeClientContext())
                {
                    dc.BadNicknames.RemoveRange(dc.BadNicknames);
                    dc.BadNicknames.AddRange(
                        badNicknames.Distinct().Select(x => new BadNickname() { Text = x }).ToArray());
                    dc.SaveChanges();
                }
            });
        }

        public Task UpdateImportantWords(string[] importantWords)
        {
            return Task.Run(() =>
            {
                using (var dc = new Arma3BeClientContext())
                {
                    dc.ImportantWords.RemoveRange(dc.ImportantWords);
                    dc.ImportantWords.AddRange(importantWords.Distinct().Select(x => new ImportantWord() { Text = x })
                        .ToArray());
                    dc.SaveChanges();
                }
            });
        }

        public Task UpdateBanReasons(string[] banReasons)
        {
            return Task.Run(() =>
            {
                using (var dc = new Arma3BeClientContext())
                {
                    dc.BanReasons.RemoveRange(dc.BanReasons);
                    dc.BanReasons.AddRange(banReasons.Distinct().Select(x => new BanReason() { Text = x }).ToArray());
                    dc.SaveChanges();
                }
            });
        }

        public Task UpdateKickReasons(string[] kickReasons)
        {
            return Task.Run(() =>
            {
                using (var dc = new Arma3BeClientContext())
                {
                    dc.KickReasons.RemoveRange(dc.KickReasons);
                    dc.KickReasons.AddRange(kickReasons.Distinct().Select(x => new KickReason() { Text = x }).ToArray());
                    dc.SaveChanges();
                }
            });
        }

        public Task UpdateBanTimes(BanTime[] banTimes)
        {
            return Task.Run(() =>
            {
                using (var dc = new Arma3BeClientContext())
                {
                    dc.BanTimes.RemoveRange(dc.BanTimes);
                    dc.BanTimes.AddRange(banTimes.Distinct().ToArray());
                    dc.SaveChanges();
                }
            });
        }
    }
}