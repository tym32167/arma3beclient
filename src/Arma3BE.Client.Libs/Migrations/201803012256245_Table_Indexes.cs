namespace Arma3BEClient.Libs.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Table_Indexes : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Bans", new[] { "PlayerId" });
            DropIndex("dbo.Bans", new[] { "ServerId" });
            DropIndex("dbo.ChatLogs", new[] { "ServerId" });
            CreateIndex("dbo.ServerInfoes", "Active");
            CreateIndex("dbo.Bans", "PlayerId");
            CreateIndex("dbo.Bans", "ServerId");
            CreateIndex("dbo.Bans", "GuidIp");
            CreateIndex("dbo.Bans", "Minutes");
            CreateIndex("dbo.Bans", "MinutesLeft");
            CreateIndex("dbo.Bans", "CreateDate");
            CreateIndex("dbo.Bans", "IsActive");
            CreateIndex("dbo.Players", "GUID");
            CreateIndex("dbo.Players", "SteamId");
            CreateIndex("dbo.Players", "LastSeen");
            CreateIndex("dbo.ChatLogs", "Text");
            CreateIndex("dbo.ChatLogs", "ServerId");
            CreateIndex("dbo.ChatLogs", "Date");
        }
        
        public override void Down()
        {
            DropIndex("dbo.ChatLogs", new[] { "Date" });
            DropIndex("dbo.ChatLogs", new[] { "ServerId" });
            DropIndex("dbo.ChatLogs", new[] { "Text" });
            DropIndex("dbo.Players", new[] { "LastSeen" });
            DropIndex("dbo.Players", new[] { "SteamId" });
            DropIndex("dbo.Players", new[] { "GUID" });
            DropIndex("dbo.Bans", new[] { "IsActive" });
            DropIndex("dbo.Bans", new[] { "CreateDate" });
            DropIndex("dbo.Bans", new[] { "MinutesLeft" });
            DropIndex("dbo.Bans", new[] { "Minutes" });
            DropIndex("dbo.Bans", new[] { "GuidIp" });
            DropIndex("dbo.Bans", new[] { "ServerId" });
            DropIndex("dbo.Bans", new[] { "PlayerId" });
            DropIndex("dbo.ServerInfoes", new[] { "Active" });
            CreateIndex("dbo.ChatLogs", "ServerId");
            CreateIndex("dbo.Bans", "ServerId");
            CreateIndex("dbo.Bans", "PlayerId");
        }
    }
}
