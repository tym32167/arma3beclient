namespace Arma3BEClient.Libs.Migrations
{
    using System.Data.Entity.Migrations;

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
