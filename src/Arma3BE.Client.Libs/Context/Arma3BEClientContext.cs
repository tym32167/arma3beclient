using System.Data.Entity;
using System.Diagnostics;
using Arma3BEClient.Libs.Migrations;
using Arma3BEClient.Libs.ModelCompact;
using Arma3BEClient.Libs.Tools;

namespace Arma3BEClient.Libs.Context
{
    public class Arma3BeClientContext : DbContext
    {
        static Arma3BeClientContext()
        {
            // ensuring obsolette schema initialization.
            new Arma3BeClientContextObsolete.ObsoletteRepository().EnsureDatabase();

            Database.SetInitializer(new ProjectInitializer());
            using (var db = new Arma3BeClientContext())
                db.Database.Initialize(false);
        }

        public Arma3BeClientContext() : base(new ConnectionFactory().Create(), true)
        {
            //this.Database.Log = s =>
            //{
            //    Debug.WriteLine(s);
            //};
        }

        public DbSet<ChatLog> ChatLog { get; set; }
        public DbSet<Note> Comments { get; set; }
        public DbSet<Player> Player { get; set; }
        public DbSet<ServerInfo> ServerInfo { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<Ban> Bans { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<PlayerHistory> PlayerHistory { get; set; }


        public DbSet<BanReason> BanReasons { get; set; }
        public DbSet<KickReason> KickReasons { get; set; }
        public DbSet<BanTime> BanTimes { get; set; }
    }
    internal class ProjectInitializer : MigrateDatabaseToLatestVersion<Arma3BeClientContext, Configuration>
    {
    }
}