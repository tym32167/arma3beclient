using System.Data.Entity.Migrations;

namespace Arma3BEClient.Libs.EF.Migrations
{
    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once InconsistentNaming
    public partial class Added_custom_settings : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomSettings",
                c => new
                {
                    Id = c.String(nullable: false, maxLength: 400),
                    Value = c.String(),
                })
                .PrimaryKey(t => t.Id);

        }

        public override void Down()
        {
            DropTable("dbo.CustomSettings");
        }
    }
}
