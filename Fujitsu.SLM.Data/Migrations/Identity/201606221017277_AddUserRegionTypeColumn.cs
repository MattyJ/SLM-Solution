namespace Fujitsu.SLM.Data.Migrations.Identity
{
    using System.Data.Entity.Migrations;

    public partial class AddUserRegionTypeColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "RegionTypeId", c => c.Int());
            CreateIndex("dbo.AspNetUsers", "RegionTypeId");
            AddForeignKey("dbo.AspNetUsers", "RegionTypeId", "dbo.RegionTypeRefData", "Id");
        }

        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "RegionTypeId", "dbo.RegionTypeRefData");
            DropIndex("dbo.AspNetUsers", new[] { "RegionTypeId" });
            DropColumn("dbo.AspNetUsers", "RegionTypeId");
        }
    }
}
