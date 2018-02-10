using System.Data.Entity.Migrations;

namespace Arma3BEClient.Libs.EF.Migrations
{
    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once InconsistentNaming
    public partial class Added_SteamId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Players", "SteamId", c => c.String(maxLength: 4000));
        }

        public override void Down()
        {
            DropColumn("dbo.Players", "SteamId");
        }
    }
}