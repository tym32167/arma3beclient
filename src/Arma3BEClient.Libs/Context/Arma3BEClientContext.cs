using System.Data.Entity;
using Arma3BEClient.Libs.ModelCompact;

namespace Arma3BEClient.Libs.Context
{
    public class Arma3BeClientContext:DbContext
    {
        public DbSet<ChatLog> ChatLog { get; set; }
        public DbSet<Note> Comments { get; set; }
        public DbSet<Player> Player { get; set; }
        public DbSet<ServerInfo> ServerInfo { get; set; }

        public DbSet<Settings> Settings { get; set; }
        
        public DbSet<Ban> Bans { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<PlayerHistory> PlayerHistory { get; set; }

        

        public Arma3BeClientContext() : base("name=Arma3BEClientEntities")
        {
          
        }

        static Arma3BeClientContext()
        {
            Database.SetInitializer(new DbInitializer());
            using (var db = new Arma3BeClientContext())
                db.Database.Initialize(false);
        }
    }


    class DbInitializer : CreateDatabaseIfNotExists<Arma3BeClientContext>
    {
        public DbInitializer()
        {
            
        }
    } 
}