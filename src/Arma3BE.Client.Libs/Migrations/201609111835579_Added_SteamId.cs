namespace Arma3BEClient.Libs.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
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
