using System.Data.Entity.Migrations;

namespace Arma3BEClient.Libs.Migrations
{
    public partial class Kick_Ban_Times_Reasons_Migration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BanReasons",
                c => new
                {
                    Id = c.Int(false, true),
                    Text = c.String(false, 4000)
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.BanTimes",
                c => new
                {
                    Id = c.Int(false, true),
                    Title = c.String(false, 4000),
                    TimeInMinutes = c.Int(false)
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.KickReasons",
                c => new
                {
                    Id = c.Int(false, true),
                    Text = c.String(false, 4000)
                })
                .PrimaryKey(t => t.Id);
        }

        public override void Down()
        {
            DropTable("dbo.KickReasons");
            DropTable("dbo.BanTimes");
            DropTable("dbo.BanReasons");
        }
    }
}