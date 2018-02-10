using System.Data.Entity.Migrations;

namespace Arma3BEClient.Libs.EF.Migrations
{
    public partial class BadNicknames_ImportantWords : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BadNicknames",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Text = c.String(maxLength: 4000),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.ImportantWords",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Text = c.String(maxLength: 4000),
                })
                .PrimaryKey(t => t.Id);
        }

        public override void Down()
        {
            DropTable("dbo.ImportantWords");
            DropTable("dbo.BadNicknames");
        }
    }
}
