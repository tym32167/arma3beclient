using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Arma3BEClient.Libs.ModelCompact;

namespace Arma3BEClient.Libs.Context
{
    public class Arma3BeClientContext : DbContext
    {
        static Arma3BeClientContext()
        {
            Database.SetInitializer(new DbInitializer());
            using (var db = new Arma3BeClientContext())
                db.Database.Initialize(false);
        }

        public Arma3BeClientContext() : base("name=Arma3BEClientEntities")
        {
        }

        public DbSet<ChatLog> ChatLog { get; set; }
        public DbSet<Note> Comments { get; set; }
        public DbSet<Player> Player { get; set; }
        public DbSet<ServerInfo> ServerInfo { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<Ban> Bans { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<PlayerHistory> PlayerHistory { get; set; }
    }


    internal class DbInitializer : CreateDatabaseIfNotExists<Arma3BeClientContext>
    {
    }

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

        public void Dispose()
        {
        }
    }
}