using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Arma3BE.Server.Models;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Helpers.Views;
using Arma3BEClient.Libs.Context;

namespace Arma3BEClient.Helpers
{
    public class BanHelper : StateHelper<Ban>
    {
        private readonly Guid _currentServerId;
        private readonly ILog _log;
        private readonly Regex replace = new Regex(@"\[[^\]^\[]*\]", RegexOptions.Compiled | RegexOptions.Multiline);

        public BanHelper(ILog log, Guid currentServerId)
        {
            _log = log;
            _currentServerId = currentServerId;
        }

        public bool RegisterBans(IEnumerable<Ban> list)
        {
            var c = list.ToList();
            var userIds = c.Select(x => x.GuidIp).Distinct().ToArray();

            if (!HaveChanges(c, x => x.Num)) return false;


            using (var context = new Arma3BeClientContext())
            {
                var db = context.Bans.Where(x => x.IsActive && x.ServerId == _currentServerId && userIds.Contains(x.GuidIp)).ToList();

                var ids = c.Select(x => x.GuidIp).ToList();
                ids.AddRange(db.Select(x => x.GuidIp).ToList());
                ids = ids.Distinct().ToList();

                var players = context.Player.Where(x => ids.Contains(x.GUID)).ToList();

                foreach (var ban in db)
                {
                    var actual = c.FirstOrDefault(x => x.GuidIp == ban.GuidIp);
                    if (actual == null)
                    {
                        ban.IsActive = false;
                    }
                    else
                    {
                        ban.MinutesLeft = actual.Minutesleft;
                        if (ban.PlayerId == null)
                        {
                            var player = players.FirstOrDefault(x => x.GUID == ban.GuidIp);
                            if (player != null && !string.IsNullOrEmpty(ban.Reason))
                            {
                                ban.PlayerId = player.Id;

                                var comm = ban.Reason;

                                var ind1 = comm.IndexOf('[');
                                var ind2 = comm.LastIndexOf(']');

                                if (ind1 > -1 && ind2 > ind1) comm = comm.Remove(ind1, ind2 - ind1 + 1).Trim();

                                if (string.IsNullOrEmpty(player.Comment) || !player.Comment.Contains(comm))
                                {
                                    player.Comment = $"{player.Comment} | {comm}";
                                }
                            }
                        }
                    }
                }

                foreach (var ban in c)
                {
                    var bdb = db.FirstOrDefault(x => x.ServerId == _currentServerId && x.GuidIp == ban.GuidIp);
                    if (bdb == null)
                    {
                        var player = players.FirstOrDefault(x => x.GUID == ban.GuidIp);
                        var newBan = new Libs.ModelCompact.Ban
                        {
                            CreateDate = DateTime.UtcNow,
                            GuidIp = ban.GuidIp,
                            IsActive = true,
                            Minutes = ban.Minutesleft,
                            MinutesLeft = ban.Minutesleft,
                            Num = ban.Num,
                            Reason = ban.Reason,
                            ServerId = _currentServerId,
                            Player = player
                        };

                        context.Bans.Add(newBan);

                        var comm = replace.Replace(ban.Reason, string.Empty).Trim();
                        if (player != null && (string.IsNullOrEmpty(player.Comment) || !player.Comment.Contains(comm)))
                        {
                            player.Comment = $"{player.Comment} | {comm}";
                        }
                    }
                }

                context.SaveChanges();
            }

            return true;
        }

        public IEnumerable<BanView> GetBanView(IEnumerable<Ban> list)
        {
            using (var context = new Arma3BeClientContext())
            {
                var guids = list.Select(x => x.GuidIp).ToList();
                var players = context.Player.Where(x => guids.Contains(x.GUID)).ToList();

                return list.Select(x =>
                {
                    var p = players.FirstOrDefault(y => y.GUID == x.GuidIp);

                    var ban = new BanView
                    {
                        Num = x.Num,
                        GuidIp = x.GuidIp,
                        Reason = x.Reason,
                        Minutesleft = x.Minutesleft
                    };

                    if (p != null)
                    {
                        ban.PlayerComment = p.Comment;
                        ban.PlayerName = p.Name;
                    }
                    return ban;
                }).ToList();
            }
        }
    }
}