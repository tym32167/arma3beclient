using System.Collections.Generic;
using System.Linq;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Libs.Context;
using Arma3BEClient.Libs.ModelCompact;
using Arma3BEClient.Updater.Models;
using Admin = Arma3BEClient.Updater.Models.Admin;

namespace Arma3BEClient.Helpers
{
    public class AdminHelper : StateHelper<Admin>
    {
         private readonly ServerInfo _currentServer;

         public AdminHelper(ILog log, ServerInfo currentServer)
        {
            _currentServer = currentServer;
        }

        public bool RegisterAdmins(IEnumerable<Admin> list)
        {
            var l = list.ToList();

            var ips = l.Select(x => x.IP).ToList();

            using (var context = new Arma3BeClientContext())
            {
                var adminsdb = context.Admins.Where(x => x.ServerId == _currentServer.Id && ips.Contains(x.IP)).ToList();

                foreach (var admin in l)
                {
                    var db = adminsdb.FirstOrDefault(x => x.IP == admin.IP/* && x.Port == admin.Port*/);
                    if (db == null)
                    {
                        context.Admins.Add(new Libs.ModelCompact.Admin()
                        {
                            ServerId = _currentServer.Id,
                            IP = admin.IP,
                            Port = admin.Port,
                            Num = admin.Num,
                        });
                    }
                }

                context.SaveChanges();

            }

            return true;
        } 
    }
}