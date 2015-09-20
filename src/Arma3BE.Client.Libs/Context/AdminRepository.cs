using System;
using System.Collections.Generic;
using System.Linq;
using Admin = Arma3BE.Server.Models.Admin;

namespace Arma3BEClient.Libs.Context
{
    public class AdminRepository : IDisposable
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
    }
}