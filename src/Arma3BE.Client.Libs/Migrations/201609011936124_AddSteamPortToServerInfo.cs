using System.Data.Entity.Migrations;

namespace Arma3BEClient.Libs.Migrations
{
    // ReSharper disable once UnusedMember.Global
    public partial class AddSteamPortToServerInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServerInfoes", "SteamPort", c => c.Int(false, defaultValue: 2303));
        }

        public override void Down()
        {
            DropColumn("dbo.ServerInfoes", "SteamPort");
        }
    }
}