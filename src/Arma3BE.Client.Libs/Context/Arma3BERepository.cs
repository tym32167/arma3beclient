using System;
using System.Collections.Generic;
using System.Linq;
using Arma3BE.Server.Models;
using Arma3BEClient.Libs.ModelCompact;
using Admin = Arma3BE.Server.Models.Admin;
using Ban = Arma3BEClient.Libs.ModelCompact.Ban;

namespace Arma3BEClient.Libs.Context
{
    public class Arma3BERepository : IDisposable
    {
        public void Dispose()
        {
        }

        public void AddOrUpdate(IEnumerable<Admin> admins, Guid serverId)
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
                        context.Admins.Add(new ModelCompact.Admin
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


        

        public IQueryable<ChatLog> GetChatLogs(string selectedServers, DateTime? startDate, DateTime? endDate,
            string filter)
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

        

        public IEnumerable<Ban> GetActivePermBans()
        {
            using (var dc = new Arma3BeClientContext())
            {
                return dc.Bans.Where(x => x.ServerInfo.Active && x.IsActive && x.MinutesLeft == 0).ToList();
            }
        }

        
    }
}