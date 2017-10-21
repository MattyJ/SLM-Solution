namespace Fujitsu.SLM.Data.Migrations.Identity
{
    using System.Data.Entity.Migrations;

    public partial class MakeUserRegionTypeMandatory : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AspNetUsers", "RegionTypeId", "dbo.RegionTypeRefData");
            DropIndex("dbo.AspNetUsers", new[] { "RegionTypeId" });
            AlterColumn("dbo.AspNetUsers", "RegionTypeId", c => c.Int(nullable: false));
            CreateIndex("dbo.AspNetUsers", "RegionTypeId");
            AddForeignKey("dbo.AspNetUsers", "RegionTypeId", "dbo.RegionTypeRefData", "Id", cascadeDelete: true);
        }

        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "RegionTypeId", "dbo.RegionTypeRefData");
            DropIndex("dbo.AspNetUsers", new[] { "RegionTypeId" });
            AlterColumn("dbo.AspNetUsers", "RegionTypeId", c => c.Int());
            CreateIndex("dbo.AspNetUsers", "RegionTypeId");
            AddForeignKey("dbo.AspNetUsers", "RegionTypeId", "dbo.RegionTypeRefData", "Id");
        }
    }
}
