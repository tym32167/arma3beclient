using System.Configuration;
using Arma3BEClient.Common.Logging;
using Arma3BEClient.Libs.ModelCompact;
using Arma3BEClient.Libs.Tools;
using System.Data.Entity;
using System.Diagnostics;
using Configuration = Arma3BEClient.Libs.Migrations.Configuration;

// ReSharper disable UnusedAutoPropertyAccessor.Global

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

        private readonly ILog _log = LogFactory.Create(new StackTrace().GetFrame(0).GetMethod().DeclaringType);
        public Arma3BeClientContext() : base(new ConnectionFactory().Create(), true)
        {
            var debugServerKey = "DebugServerEnabled";
            //if (ConfigurationManager.AppSettings[debugServerKey] == bool.TrueString)
            {
                this.Database.Log = s =>
                {
                    _log.Info(s);
                };
            }
        }

        public DbSet<ChatLog> ChatLog { get; set; }
        public DbSet<Note> Comments { get; set; }
        public DbSet<Player> Player { get; set; }
        public DbSet<ServerInfo> ServerInfo { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<CustomSettings> CustomSettings { get; set; }
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