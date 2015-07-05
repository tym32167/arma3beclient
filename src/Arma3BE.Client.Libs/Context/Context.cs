using System.Linq;
using Arma3BEClient.Libs.ModelCompact;

namespace Arma3BEClient.Libs.Context
{
    public class Context : Arma3BeClientContext, IContext
    {
        public new IQueryable<ChatLog> ChatLog { get { return base.ChatLog.AsQueryable(); } }
        public new IQueryable<Note> Comments { get { return base.Comments.AsQueryable(); } }
        public new IQueryable<Player> Player { get { return base.Player.AsQueryable(); } }
        public new IQueryable<ServerInfo> ServerInfo { get { return base.ServerInfo.AsQueryable(); } }
        public new IQueryable<Settings> Settings { get { return base.Settings.AsQueryable(); } }
        public new IQueryable<Ban> Bans { get { return base.Bans.AsQueryable(); } }
        public new IQueryable<Admin> Admins { get { return base.Admins.AsQueryable(); } }
        public new IQueryable<PlayerHistory> PlayerHistory { get { return base.PlayerHistory.AsQueryable(); } }
        
        public void Save()
        {
            SaveChanges();
        }
    }
}