using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Arma3BE.Server.Models;
using Arma3BEClient.Libs.ModelCompact;
using Admin = Arma3BEClient.Libs.ModelCompact.Admin;
using Ban = Arma3BEClient.Libs.ModelCompact.Ban;
using Player = Arma3BEClient.Libs.ModelCompact.Player;

namespace Arma3BEClient.Libs.Context
{
    public class Arma3BERepository : IDisposable
    {
        public void AddOrUpdate(IEnumerable<Arma3BE.Server.Models.Admin> admins, Guid serverId)
        {
            var l = admins.ToList();
            var ips = l.Select(x => x.IP).ToList();

            using (var context = new Arma3BeClientContext())
            {
                var adminsdb = context.Admins.Where(x => x.ServerId == serverId && ips.Contains(x.IP)).ToList();

                foreach (var admin in l)
                {
                    var db = adminsdb.FirstOrDefault(x => x.IP == admin.IP);
                    if (db == null)
                    {
                        context.Admins.Add(new Admin
                        {
                            ServerId = serverId,
                            IP = admin.IP,
                            Port = admin.Port,
                            Num = admin.Num
                        });
                    }
                }

                context.SaveChanges();
            }
        }

        public void AddOrUpdate(ChatMessage message, Guid serverId)
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
        }

        public IEnumerable<ServerInfo> GetActiveServerInfo()
        {
            using (var dc = new Arma3BeClientContext())
            {
                return dc.ServerInfo.Where(x => x.Active).ToList();
            }
        }

        public IEnumerable<ServerInfo> GetServerInfo()
        {
            using (var dc = new Arma3BeClientContext())
            {
                return dc.ServerInfo.ToList();
            }
        }

        public void SetServerInfoActive (Guid serverInfoId, bool active)
        {
            using (var dc = new Arma3BeClientContext())
            {
                var server = dc.ServerInfo.FirstOrDefault(x => x.Id == serverInfoId);
                if (server != null)
                {
                    server.Active = active;
                    dc.SaveChanges();
                }
            }
        }

        public IEnumerable<ServerInfo> GetNotActiveServerInfo()
        {
            using (var dc = new Arma3BeClientContext())
            {
                return dc.ServerInfo.Where(x => !x.Active).ToList();
            }
        }

        public IEnumerable<Player> GetAllPlayers()
        {
            using (var dc = new Arma3BeClientContext())
            {
                return dc.Player.ToList();
            }
        }

        public void AddPlayers(IEnumerable<Player> players)
        {
            using (var dc = new Arma3BeClientContext())
            {
                dc.Player.AddRange(players);
                dc.SaveChanges();
            }
        }


        public IEnumerable<Player> GetPlayers(System.Linq.Expressions.Expression<Func<Player, bool>> expression)
        {
            using (var dc = new Arma3BeClientContext())
            {
                return dc.Player.Where(expression).ToList();
            }
        }


        public IQueryable<ChatLog> GetChatLogs(string selectedServers, DateTime? startDate, DateTime? endDate, string filter)
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
                return log;
            }
        }

        public Player GetPlayerInfo(string guid)
        {
            using (var dc = new Arma3BeClientContext())
            {
                return dc.Player.Where(x => x.GUID == guid).Include(x=>x.Bans).Include(x=>x.Bans.Select(b=>b.ServerInfo)).Include(x=>x.Notes).Include(x=>x.PlayerHistory).FirstOrDefault();
            }
        }

        public IEnumerable<Ban> GetActivePermBans()
        {
            using (var dc = new Arma3BeClientContext())
            {
                return dc.Bans.Where(x => x.ServerInfo.Active && x.IsActive && x.MinutesLeft == 0).ToList();
            }
        }

        public void UpdatePlayerComment(string guid, string comment)
        {
            using (var dc = new Arma3BeClientContext())
            {
                var dbp = dc.Player.FirstOrDefault(x => x.GUID == guid);
                if (dbp != null)
                {
                    dbp.Comment = comment;
                    dc.SaveChanges();
                }
            }
        }

        public void Dispose()
        {
        }
    }
}