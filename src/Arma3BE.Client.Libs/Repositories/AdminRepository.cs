using Arma3BE.Server.Models;
using Arma3BEClient.Common.Core;
using Arma3BEClient.Libs.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arma3BEClient.Libs.Repositories
{
    public class AdminRepository : DisposeObject
    {
        public Task AddOrUpdateAsync(IEnumerable<Admin> admins, Guid serverId)
        {
            return Task.Run(() =>
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
            });
        }
    }
}